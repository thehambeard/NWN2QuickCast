﻿using Kingmaker.Utility;
using Owlcat.Runtime.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;

namespace NWN2QuickCast.UI.MVVM.VMs.Elements
{
    public abstract class ElementBaseVM : VirtualListElementVMBase
    {
        public IReadOnlyCollection<ElementBaseVM> Children => _children;
        public IReadOnlyReactiveProperty<bool> IsExpanded => _isExpanded;
        public bool HasChildren => _children.Count > 0;

        private readonly ReactiveCollection<ElementBaseVM> _children = new ReactiveCollection<ElementBaseVM>();
        private readonly BoolReactiveProperty _isExpanded = new BoolReactiveProperty(true);

        public ElementBaseVM(List<ElementBaseVM> children = null)
        {
            if (children != null)
                children.ForEach(x => AddChild(x));
        }

        public void AddChild(ElementBaseVM child) =>
            _children.Add(child);

        public void ExpandChildren()
        {
            foreach (var child in _children)
            {
                child.Active.Value = true;
                if (child.HasChildren && child.IsExpanded.Value)
                    child.ExpandChildren();
            }

            _isExpanded.Value = true;
        }

        public void ExpandAllChildren()
        {
            foreach (var child in _children)
            {
                if (child.HasChildren)
                    ExpandAllChildren();

                ExpandChildren();
            }
        }

        public void CollaspeChildren()
        {
            foreach(var child in _children)
            {
                if (child.HasChildren)
                    child.CollaspeChildrenPreserve();

                child.Active.Value = false;
            }

            _isExpanded.Value = false;
        }

        private void CollaspeChildrenPreserve()
        {
            foreach (var child in _children)
            {
                if (child.HasChildren)
                    child.CollaspeChildren();

                child.Active.Value = false;
            }
        }

        public void CollaspeAllChildren()
        {
            foreach (var child in _children)
            {
                if (child.HasChildren)
                    CollaspeAllChildren();

                CollaspeChildren();
            }
        }

        public void ToggleExpandCollapse()
        {
            if (_isExpanded.Value)
                CollaspeChildren();
            else
                ExpandChildren();
        }

        public List<ElementBaseVM> Flatten(bool includeRoot = true)
        {
            var result = new List<ElementBaseVM>();
            Traverse(this, result, includeRoot);
            return result;
        }

        private void Traverse(ElementBaseVM node, List<ElementBaseVM> result, bool includeRoot = true)
        {
            if (includeRoot)
                result.Add(node);

            if (node.HasChildren)
                foreach (var child in node.Children)
                    Traverse(child, result);
        }

        public override void DisposeImplementation()
        {
        }
    }
}
