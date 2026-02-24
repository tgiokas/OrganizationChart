namespace ExternalIntegrations.OrganizationChart.Application.Errors;

public static class ErrorCodes
{
    public static class IMPORT
    {
        public const string UnexpectedError = "IMP-000";
        public const string JobNotFound = "IMP-001";
        public const string EmptyBatchRequest = "IMP-002";
        public const string EmptyDeltaRequest = "IMP-003";
        public const string InvalidExternalId = "IMP-004";
        public const string DuplicateExternalId = "IMP-005";
        public const string AuthApiCreateFailed = "IMP-006";
        public const string AuthApiUpdateFailed = "IMP-007";
        public const string AuthApiDisableFailed = "IMP-008";
        public const string MissingRequiredFields = "IMP-009";
        public const string PartnerCodeRequired = "IMP-010";
        public const string JobAlreadyProcessing = "IMP-011";
    }
}
