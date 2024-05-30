using System.Collections.Generic;
using UmbNav.Core.Models;

namespace UmbNav.Core.Interfaces
{
    public interface IUmbNavMenuBuilderService
    {
        IEnumerable<UmbNavItem> BuildMenu(IEnumerable<UmbNavItem> items, int level = 0, bool removeNaviHideItems = false,
            bool removeNoopener = false, bool removeNoreferrer = false, bool removeIncludeChildNodes = false, bool allowMenuItemDescriptions = false);
    }
}
