namespace IntegrationImport.Domain.Enums;

public enum ImportJobStatus
{
    PENDING_REVIEW = 1,
    UNDER_REVIEW = 2,
    REVIEW_COMPLETED = 3,
    PARTIALLY_APPLIED = 4,
    APPLIED = 5
}