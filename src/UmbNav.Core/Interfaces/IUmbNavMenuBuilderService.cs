using System.Collections.Generic;
using UmbNav.Core.Models;

namespace UmbNav.Core.Interfaces
{
    public interface IUmbNavMenuBuilderService
    {
        IEnumerable<UmbNavInternalItem> BuildMenu(IEnumerable<UmbNavInternalItem> items, int level = 0, bool removeNaviHideItems = false,
            bool removeNoopener = false, bool removeNoreferrer = false, bool removeIncludeChildNodes = false);

        IEnumerable<UmbNavItem> BuildRenderingMenu(IEnumerable<UmbNavInternalItem> internalItems);
    }
}
