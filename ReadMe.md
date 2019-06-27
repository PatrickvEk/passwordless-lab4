# Connecting on-prem to keyvault



Create of reuse a keyvault from the previous labs and **note the keyvault-name**.













-------- Remove, just for styling reference.





**Visit your website** again, you should get an **error now** since the website is not yet connected to the virtual network.



So let's connect the **website**. Go to the **Networking** part of the website and **configure vnet integration**.

![Connect website](images/website-networking.PNG)



**Connect** to the Vnet.

![Connect website](images/vnet integration.PNG)

## Configure the Vnet

We need to enable the **storage endpoint** on the VNet.

Go back to the **storageaccount** in the portal, go to **Firewall and virtual networks**.

Click it and, open the "Default" **subnet** where it says **"Endpoint disabled"**.

Go to the "**service endpoints section**"
Tick the box on the **Microsoft.Storage** and press save.

![enable service enpoint](images/enable-service-endpoint.PNG)



## Done

Now it might take a while for azure to configure all your settings and networking.

You can try **restarting** your website and check if it works again in isolated mode by visiting it.