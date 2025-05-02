using Ofgem.LAF.SharedLibrary.Models;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.Abstractions;

public interface ILocalAuthorityRepository
{
    Task<ICollection<Ofgem.LAF.SharedLibrary.Models.LocalAuthority>> GetAll();

    Task<ICollection<Ofgem.LAF.SharedLibrary.Models.LocalAuthority>> GetAssociatedLocalAuthorities(string onsCode);

    Task<Ofgem.LAF.SharedLibrary.Models.LocalAuthority?> Get(Guid localAuthorityId);

    Task<Ofgem.LAF.SharedLibrary.Models.LocalAuthority?> GetByOnsCode(string onsCode);

    Task<Ofgem.LAF.SharedLibrary.Models.LocalAuthority?> GetByName(string name);

    Task<PagedResult<Ofgem.LAF.SharedLibrary.Models.LocalAuthority>?> GetFilteredLocalAuthorities(ProfilesFilter filter);

    Task<Ofgem.LAF.SharedLibrary.Models.LocalAuthority> Create(Ofgem.LAF.SharedLibrary.Models.LocalAuthority localAuthority);

    Task<Ofgem.LAF.SharedLibrary.Models.LocalAuthority?> Update(string name, Guid localAuthorityId);

    Task<Ofgem.LAF.SharedLibrary.Models.LocalAuthority?> UpdateByOnsCode(string onsCode, string name, string email);

    Task Delete(Guid localAuthorityId);


}