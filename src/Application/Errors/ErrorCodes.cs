namespace IntegrationImport.Application.Errors;

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
        public const string ImportRequestNotFound = "IMP-012";
        public const string SyncIdRequired = "IMP-013";
        public const string AtLeastOneItemReviewDecisionRequired = "IMP-014";
        public const string UserImportExists = "IMP-015";
        public const string ItemNotFound = "IMP-016";
        public const string ItemNotApproved = "IMP-017";
        public const string ItemStatusNotPendingDispatch = "IMP-018";
        public const string ItemDoesNotBelongToSync = "IMP-019";
        public const string ItemNotInPendingReviewStatus = "IMP-020";
        public const string DuplicateItemDecisionsDetected = "IMP-021";
        public const string AnImportErrorOccurredInJson = "IMP-022";
        public const string OrgUnitHrmsIdNotFound = "IMP-023";
        public const string ValidationFailed = "IMP-024";
        public const string AllItemsRejected = "IMP-025";
        public const string OrgUnitImportExists = "IMP-026";
    }
}
