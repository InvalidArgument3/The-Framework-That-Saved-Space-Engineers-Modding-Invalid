﻿using System;
using System.Collections;
using System.Collections.Generic;
using VRage;
using ApiMemberAccessor = System.Func<object, int, object>;

namespace RichHudFramework
{
    using ControlContainerMembers = MyTuple<
        ApiMemberAccessor, // GetOrSetMember,
        MyTuple<object, Func<int>>, // Member List
        object // ID
    >;

    namespace UI.Client
    {
        /// <summary>
        /// Vertically scrolling collection of control categories.
        /// </summary>
        public class ControlPage : TerminalPageBase, IControlPage
        {
            /// <summary>
            /// List of control categories registered to the page.
            /// </summary>
            public IReadOnlyList<ControlCategory> Categories { get; }

            public IControlPage<ControlCategory, ControlTile> CategoryContainer => this;

            public ControlPage() : base(ModPages.ControlPage)
            {
                var catData = (MyTuple<object, Func<int>>)GetOrSetMemberFunc(null, (int)ControlPageAccessors.CategoryData);
                var GetCatDataFunc = catData.Item1 as Func<int, ControlContainerMembers>;

                Func<int, ControlCategory> GetCatFunc = (x => new ControlCategory(GetCatDataFunc(x)));
                Categories = new ReadOnlyApiCollection<ControlCategory>(GetCatFunc, catData.Item2);
            }

            IEnumerator<ControlCategory> IEnumerable<ControlCategory>.GetEnumerator() =>
                Categories.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() =>
                Categories.GetEnumerator();

            /// <summary>
            /// Adds the given control category to the page.
            /// </summary>
            public void Add(ControlCategory category) =>
                GetOrSetMemberFunc(category.ID, (int)ControlPageAccessors.AddCategory);
        }
    }
}