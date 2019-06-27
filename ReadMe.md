# Connecting on-prem to KeyVault



In this lab you'll be able to integrate keyvault in your application in a 'codeless' way. We'll do that via dll-sideloading. This means there is **no need to change code** in your existing application. We'll only add the keyvault connector modules and change the web.config.



Create of reuse a keyvault from the previous labs and **note the keyvault-name**.

**Clone the repository** to your local disk.



These are the web.config values we'll need to collect:

    <add key="ClientId" value="421dc308-105d-4d62-9b06-d1317547363e" />
    <add key="ClientCertificate" value="PatrickE01.KeyVault.local" />
    
    <!-- ClientSecret is required when ClientCertificate is not used -->
    <!-- <add key="ClientSecret" value="UMC/pckdl3tlEGhLbsgBNP5IAKcC1y24Y0len83UA/8=" /> -->
    
    <!-- SecretUri is the URI for the secret in Azure Key Vault -->
    <add key="SecretUri" value="https://meetup-keyvault.vault.azure.net/secrets/DatabasePassword" />


These might also need changing.

    <!-- database connection settings in conjunction with keyvault -->
    <add key="ConnectionStringName" value="ContosoUniversity.DAL.SchoolContext_KeyVault" />
    <add key="RunTestQuery" value="true" />


First **generate and install your client certificate**.

Run this powershell:

```powershell
#Create self-signed certificate and export pfx and cer files 
$PfxFilePath = 'KVWebApp.pfx'
$CerFilePath = 'KVWebApp.cer'
$DNSName = 'yourname.KeyVault.local' #you'll need this value in your web.config
$Password = 'YourSuperStrongPassword'

$StoreLocation = 'CurrentUser' #be aware that LocalMachine requires elevated privileges
$CertBeginDate = Get-Date
$CertExpiryDate = $CertBeginDate.AddYears(3)

$SecStringPw = ConvertTo-SecureString -String $Password -Force -AsPlainText 
$Cert = New-SelfSignedCertificate -DnsName $DNSName -CertStoreLocation "cert:\$StoreLocation\My" -NotBefore $CertBeginDate -NotAfter $CertExpiryDate -KeySpec Signature
Export-PfxCertificate -cert $Cert -FilePath $PFXFilePath -Password $SecStringPw 
Export-Certificate -cert $Cert -FilePath $CerFilePath 

$base64 = [convert]::tobase64string((get-item cert:\currentuser\my\$($Cert.Thumbprint)).RawData)

Write-Host 
Write-Host "Public part of certificate in base64, save this, because you'll need this later:"
Write-Output $base64
```



Your **ClientCertificate** is now **generated** **and** **installed** in your windows certificate store.

Save the value of `$DNSName` and the base64 output of the public part of the certificate.



Next up, openup your **CloudShell** in the azure portal.

Change the variables in the script below.

Paste and run the script in cloudshell.

```bash
appName='ContosoUniversity'
keyVaultName='<your keyvault name>'

certificatePublicBase64='<your base64 output, without enters>'
certificateEndDate='25-6-2022'


#create app registration in azure active directory (and capture appId)
appId=$(az ad app create --display-name "$appName" --key-type AsymmetricX509Cert --key-value $certificatePublicBase64 --end-date $certificateEndDate | jq -r '.appId')
echo $appId

#create service principal
az ad sp create --id $appId >/dev/null

#give app access to keyvault (via service principal)
az keyvault set-policy --name $keyVaultName --secret-permissions get --spn $appId >/dev/null

#generate password
password=$(openssl rand 15 -base64)

#set password
az keyvault secret set --name ContosoUniversityDatabase --vault-name $keyVaultName --description "password used to acces the Contoso database" --value "$password" >/dev/null

#list password ids for easy copy/paste
az keyvault secret list --vault-name $keyVaultName | jq -r '.[].id'
```

This script sets an **App Registration** in **Azure Active Directory**. Then also in Azure Active Directory it creates a **Service Principal** onto the App Registration. The **Access Policy** will be set to **grant permission** to the keyvault. Lastly a password is generated and set into a **KeyVault-Secret**.



For easy copy/paste the `$appId` variable is printed out as well as the **secret-uris**. You'll need the `$appId` for the `ClientId` setting in web.config and **`ContosoUniversityDatabase` secretUri** for the `SecretUri` setting in your **web.config**.



To make sure EntityFramework won't be able to resolve the context we'll need to change the `SchoolContext` ConnectionString name to `ContosoUniversity.DAL.SchoolContext_KeyVault`. Now the ` <defaultConnectionFactory type="KeyVaultConnectionProvider.KeyVaultConnectionFactory, KeyVaultConnectionProvider"/>` will be used so we can process the connectionstrings and add the KeyVault secret into that.

 

# Running your app

To run your app and have **matching passwords**, make sure to rotate your password **at least once**.

This can be done by **tweaking the powershell** and running it. Or by changing the database password **manually** and updating the same password in keyvault.



```powershell
##NOTE: YOUR NEED TO TWEAK THIS SCRIPT YOURSELF TO GET YOUR KEY-ROTATION TO WORK AS PART OF THE LAB EXPERIENCE



#login to azure if not logged in already. If you are already logged in you can comment this line out.
#Login-AzureRmAccount




#https://ilovepowershell.com/2018/05/28/awesome-and-simple-way-to-generate-random-passwords-with-powershell/
Add-Type -AssemblyName System.Web 
# It just takes two arguments: "How long is the password" and "How many special characters"?
# The generated password is a string, and that's find for reading but not useful for creating a credential object. So you'll usually need to convert the password into a SecureString.
 
# Same as above, just a different password length and complexity

Write-Output "Generating password..."
$generatedPassword = [System.Web.Security.Membership]::GeneratePassword(30,10)


$newSecret = ConvertTo-SecureString -AsPlainText -Force $generatedPassword 


Write-Output "Setting password in server..."

#SERVERS: remove whichever you like.
#azure server
Set-AzureRmSqlServer -ResourceGroupName meetup-shared -ServerName meetup-shared -SqlAdministratorPassword $newSecret | out-null

#local server
$userName = 'localdb_user'

$createLogin = @"
CREATE LOGIN $userName WITH PASSWORD = '$generatedPassword';
CREATE USER $userName FOR LOGIN $userName;
EXEC sp_addrolemember 'db_owner', '$userName'
"@

$alterLogin = @"
Alter LOGIN $userName WITH PASSWORD = '$generatedPassword';
"@ 

Invoke-Sqlcmd -ConnectionString "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ContosoUniversity2" -Query $alterLogin


Write-Output "Setting password in KeyVault..."
Set-AzureKeyVaultSecret -Name DatabasePassword -SecretValue $newSecret -VaultName pvekeyvaulttest | out-null


Write-Output "New password set in Server and KeyVault!"
```



**Run your app** and debug to see what is happening. `KeyVaultConnectionFactory` is a good class to put your first breakpoint. If you have done correctly, the database connection 'just works'. Als after a keyrotation the app won't go offline and still works.





# Done

You have now learned how to to use your **on-prem app in conjuction with Azure KeyVault** in a **codeless** way. You can now automate and schedule the **keyrotation** through **on-prem taskmanagers**.



# Note

If your application isn't using EntityFramework or uses another storage system you can use the HttpModule to alter the settings in memory before the application starts. Through this way you can still use KeyVault in conjunction with your application. You might need to restart the app after a key-rotation though.

