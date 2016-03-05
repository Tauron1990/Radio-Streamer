using System;
using System.Collections.Generic;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.UI
{
    [PublicAPI]
    public static class MenuItemService
    {
        public const string NotifyContextMenuName = "NotifyContextMenu";

        private static readonly Dictionary<string, List<GenericMenuItem>> MenuItemRegistry = Initialize();

        [NotNull]
        private static Dictionary<string, List<GenericMenuItem>> Initialize()
        {
            var result = new Dictionary<string, List<GenericMenuItem>>
            {
                {NotifyContextMenuName, new List<GenericMenuItem>()}
            };

            return result;
        }

        [NotNull]
        public static Action<GenericMenuItem> RegisterMenu([NotNull] string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            var list = new List<GenericMenuItem>();
            MenuItemRegistry.Add(name, list);

            return item => GenericAdd(list, item);
        }

        [NotNull]
        public static IEnumerable<GenericMenuItem> GetMenu([NotNull] string name)
        {
            return MenuItemRegistry[name].AsReadOnly();
        }

        public static void RegisterNotify([NotNull] GenericMenuItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            AddItem(NotifyContextMenuName, item);
        }

        public static void AddItem([NotNull] string name, [NotNull] GenericMenuItem menuItem)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (menuItem == null) throw new ArgumentNullException(nameof(menuItem));
            GenericAdd(MenuItemRegistry[name], menuItem);
        }

        private static void GenericAdd([NotNull] List<GenericMenuItem> list, [NotNull] GenericMenuItem item)
        {
            var temp = list.Find(i => i.Id == item.Id);

            if (temp != null)
                Intigrate(temp, item);
            else 
                list.Add(item);
        }

        private static void Intigrate([NotNull] GenericMenuItem parent, [NotNull] GenericMenuItem item)
        {
            if (parent == null) throw new ArgumentNullException("parent");
            if (item == null) throw new ArgumentNullException("item");

            parent.ClickActions.AddRange(item.ClickActions);

            if (item.MenuItems == null) return;

            foreach (var genericMenuItem in item.MenuItems)
            {
                GenericAdd(parent.MenuItems, genericMenuItem);
            }
        }
    }
}