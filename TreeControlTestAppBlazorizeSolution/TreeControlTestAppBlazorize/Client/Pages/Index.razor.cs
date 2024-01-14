using Microsoft.JSInterop;
using System.Collections.ObjectModel;

namespace TreeControlTestAppBlazorize.Client.Pages
{
    public partial class Index
    {

        protected override void OnInitialized()
        {
            Items = new ObservableCollection<TreeItemData>();

            var item1 = new TreeItemData
            {
                Text = "Users",
                Children = new ObservableCollection<TreeItemData>(),
                Parent = null,
                IconName = "fa-users"
            };
            Items.Add(item1);
            var item2 = new TreeItemData
            {
                Text = $"User A",
                Children = new ObservableCollection<TreeItemData>(),
                Parent = item1,
                IconName = "fa-user"
            };
            item1.Children.Add(item2);
        }

        private Task OnSelectedChanged(TreeItemData selectedNode)
        {
            _selectedNode = selectedNode;
            return Task.CompletedTask;
        }
        private void OnNodeClick(TreeItemData node)
        {
            // Handle node click event (if needed)
            _selectedNode = node;
        }

        private async Task<ObservableCollection<TreeItemData>?> GetChildNodes(TreeItemData parentNode)
        {
            TreeItemData[] savedChildren = new TreeItemData[parentNode.Children.Count];

            parentNode.Children.CopyTo(savedChildren, 0);
            parentNode.Children.Clear();
            parentNode.Display = "";
            await Task.Delay(250);
            foreach (var child in savedChildren)
            {
                parentNode.Children.Add(child);
            }
            parentNode.Display = "display:none";
            return parentNode?.Children;
        }

        /// <summary>
        /// Finds a node in the tree with the given search text.
        /// </summary>
        /// <param name="nodes">The collection of nodes to search.</param>
        /// <param name="searchText">The text to search for.</param>
        /// <returns>The node with the matching text, or null if not found.</returns>
        public TreeItemData? FindNode(ObservableCollection<TreeItemData> nodes, string searchText)
        {
            foreach (var node in nodes)
            {
                if (node.Text == searchText)
                {
                    return node;  // Found the node
                }

                if (node.Children != null && node.Children.Count > 0)
                {
                    var foundNode = FindNode(node.Children, searchText);
                    if (foundNode != null)
                    {
                        return foundNode;  // Found the node in a subtree
                    }
                }
            }

            return null;  // Node not found
        }

        private async Task ContextMenu_MenuOperation(object? sender, string action)
        {
            switch (action)
            {
                case "add":
                    {
                        await OnAddNodeClick();
                        break;
                    }
                case "remove":
                    {
                        await OnDeleteNodeClick();
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        private Task OnAddNodeClick()
        {
            if (_selectedNode != null)
            {
                if (_selectedNode.Children == null)
                {
                    _selectedNode.Children = new()
                    {
                        new TreeItemData
                        {
                            Text = $"User {DateTime.Now:s}",
                            Parent = _selectedNode,
                            Children = new(),
                            IconName = _selectedNode.IconName
                        }
                    };
                }
                else
                {
                    _selectedNode.Children.Insert(0,new TreeItemData
                    {
                        Text = $"User {DateTime.Now.ToString("s")}",
                        Children = new(),
                        Parent = _selectedNode,
                        IconName = "fa-user"
                    });
                }
            }

            return Task.CompletedTask;
        }

        private Task OnDeleteNodeClick()
        {
            if (_selectedNode?.Parent?.Children?.Count > 0)
            {
                _selectedNode.Parent.Children.Remove(_selectedNode);
            }
            else if (_selectedNode != null)
            {
                Items.Remove(_selectedNode);
            }

            _selectedNode = null;

            return Task.CompletedTask;
        }

        public class TreeItemData
        {
            public string? Text { get; set; }
            public ObservableCollection<TreeItemData> Children { get; set; } = [];
            public TreeItemData Parent { get; set; }
            public string IconName { get; set; } = "";
            public bool Editable { get; set; } = false;
            public string Display { get; set; } = "display:none";
        }

        ObservableCollection<TreeItemData> Items;
        TreeItemData? _selectedNode;
        Blazorise.TreeView.TreeView<TreeItemData> _treeViewControl;
    }
}