namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.Abstractions;

public interface IDeclarationsRepository
{
    Task<int> GetCountUsingSoiId(Guid statementOfIntentId);

    int GetCountAffectedByChange(string onsCode);
}