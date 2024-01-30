using System;

namespace RPG.Stats.Relations {
    public interface IOrganisationWrapper {
        Organisation GetOrganisation();
        Guid GetGuid();
    }
}
