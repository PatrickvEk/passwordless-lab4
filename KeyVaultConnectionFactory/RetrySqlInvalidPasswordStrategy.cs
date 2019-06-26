using System;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

namespace KeyVaultConnectionProvider
{
    public class RetrySqlInvalidPasswordStrategy: DbExecutionStrategy
    {
        private readonly Action<Exception> _afterFailedAttemptCallback;
        private const int LoginFailedErrorNumber = 18456;

        public RetrySqlInvalidPasswordStrategy(Action<Exception> afterFailedAttemptCallback)
        {
            _afterFailedAttemptCallback = afterFailedAttemptCallback;
        }

        protected override bool ShouldRetryOn(Exception exception)
        {
            bool invalidPassword = exception is SqlException lastSqlError && lastSqlError.Number == LoginFailedErrorNumber;

            return invalidPassword;
        }

        protected override TimeSpan? GetNextDelay(Exception lastException)
        {
            // this means the previous execution failed

            _afterFailedAttemptCallback.Invoke(lastException);

            return base.GetNextDelay(lastException);
        }
    }
}