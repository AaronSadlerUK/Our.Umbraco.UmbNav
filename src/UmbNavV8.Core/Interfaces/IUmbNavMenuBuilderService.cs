using System.Collections.Generic;
using UmbNavV8.Core.Models;

namespace UmbNavV8.Core.Interfaces
{
    public interface IUmbNavMenuBuilderService
    {
        IEnumerable<UmbNavItem> BuildMenu(IEnumerable<UmbNavItem> items, int level = 0, bool removeNaviHideItems = false,
            bool removeNoopener = false, bool removeNoreferrer = false, bool removeIncludeChildNodes = false);
    }
}
