using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;

public abstract class ItemContainer : IItemContainer
{
	private ItemContainerCollection items;

	protected ItemContainer()
	{
		this.items = new ItemContainerCollection(this);
		this.ProjectItems = new ProjectItemsWrapper(this);
	}

	public virtual string Id { get; set; }

	public virtual string Name { get; set; }

	public virtual Icon Icon { get; set; }

	public virtual string PhysicalPath { get; set; }

	public virtual bool IsSelected { get; set; }

	public abstract ItemKind Kind { get; }

	public void Select()
	{
		this.IsSelected = true;
	}

	public virtual IList<IItemContainer> Items
	{
		get { return this.items; }
	}

	public virtual IItemContainer Add(string name, ITemplate theTemplate)
	{
		return theTemplate.Unfold(name, this);
	}

	IEnumerable<IItemContainer> IItemContainer.Items
	{
		get { return this.Items; }
	}

	public virtual IItemContainer Parent { get; set; }

	public T As<T>() where T : class
	{
		return this as T;
	}

	public dynamic ProjectItems { get; set; }

	public class ProjectItemsWrapper
	{
		private ItemContainer container;

		public ProjectItemsWrapper(ItemContainer container)
		{
			this.container = container;
		}

		public object AddFromFile(string fileName)
		{
			this.container.items.Add(new Item { Name = Path.GetFileName(fileName), PhysicalPath = fileName });

			return null;
		}
	}
}

public class DynamicProperties : DynamicObject
{
	private Dictionary<string, object> values = new Dictionary<string, object>();

	public override bool TrySetMember(SetMemberBinder binder, object value)
	{
		this.values[binder.Name] = value;
		return true;
	}

	public override bool TryGetMember(GetMemberBinder binder,
		out object result)
	{
		this.values.TryGetValue(binder.Name, out result);
		return true;
	}
}

public class Solution : ItemContainer, ISolution
{
	public override ItemKind Kind
	{
		get { return ItemKind.Solution; }
	}

	public virtual bool IsOpen
	{
		get { return false; }
	}

	public Solution()
	{
		this.Data = new DynamicProperties();
	}

	public virtual ISolutionFolder CreateSolutionFolder(string name)
	{
		var folder = new SolutionFolder();
		folder.Parent = this;
		folder.Name = name;
		this.Items.Add(folder);
		return folder;
	}

	public dynamic Data { get; private set; }

	public IEnumerable<ISolutionFolder> SolutionFolders
	{
		get { return this.Items.OfType<ISolutionFolder>(); }
	}
}

public class SolutionFolder : ItemContainer, ISolutionFolder
{
	public override ItemKind Kind
	{
		get { return ItemKind.SolutionFolder; }
	}

	public SolutionFolder()
	{
	}

	public virtual ISolutionFolder CreateSolutionFolder(string name)
	{
		var folder = new SolutionFolder();
		folder.Parent = this;
		folder.Name = name;
		this.Items.Add(folder);
		return folder;
	}

	public IEnumerable<ISolutionFolder> SolutionFolders
	{
		get { return this.Items.OfType<ISolutionFolder>(); }
	}
}

public class Project : ItemContainer, IProject
{
	public virtual string FullPath { get; set; }

	public override ItemKind Kind
	{
		get { return ItemKind.Project; }
	}

	public Project()
	{
		this.Data = new DynamicProperties();
		this.UserData = new DynamicProperties();
	}

	public virtual IFolder CreateFolder(string name)
	{
		var folder = new Folder();
		folder.Name = name;
		folder.Parent = this;
		this.Items.Add(folder);
		return folder;
	}

	public dynamic Data { get; private set; }

	public dynamic UserData { get; private set; }

	public IEnumerable<IFolder> Folders
	{
		get { return this.Items.OfType<IFolder>(); }
	}
}

public class Folder : ItemContainer, IFolder
{
	public override ItemKind Kind
	{
		get { return ItemKind.Folder; }
	}

	public Folder()
	{
	}

	public virtual IFolder CreateFolder(string name)
	{
		var folder = new Folder();
		folder.Name = name;
		folder.Parent = this;
		this.Items.Add(folder);

		return folder;
	}

	public IEnumerable<IFolder> Folders
	{
		get { return this.Items.OfType<IFolder>(); }
	}
}

public class Item : ItemContainer, IItem
{
	public override ItemKind Kind
	{
		get { return ItemKind.Item; }
	}

	public Item()
	{
		this.Data = new DynamicProperties();
	}

	public dynamic Data { get; internal set; }
}

public class ItemContainerCollection : IList<IItemContainer>
{
	private IItemContainer parent;
	private List<IItemContainer> items = new List<IItemContainer>();

	public ItemContainerCollection(IItemContainer parent)
	{
		this.parent = parent;
	}

	public int Count
	{
		get { return this.items.Count; }
	}

	bool ICollection<IItemContainer>.IsReadOnly
	{
		get { return false; }
	}

	public IItemContainer this[int index]
	{
		get
		{
			return this.items[index];
		}

		set
		{
			var typedItem = value as ItemContainer;
			if (typedItem != null)
			{
				typedItem.Parent = this.parent;
			}

			this.items[index] = value;
		}
	}

	public void Add(IItemContainer item)
	{
		var typedItem = item as ItemContainer;
		if (typedItem != null)
		{
			typedItem.Parent = this.parent;
		}

		this.items.Add(item);
	}

	public void Clear()
	{
		this.items.Clear();
	}

	public bool Contains(IItemContainer item)
	{
		return this.items.Contains(item);
	}

	void ICollection<IItemContainer>.CopyTo(IItemContainer[] array, int arrayIndex)
	{
		throw new NotImplementedException();
	}

	public int IndexOf(IItemContainer item)
	{
		return this.items.IndexOf(item);
	}

	public void Insert(int index, IItemContainer item)
	{
		var typedItem = item as ItemContainer;
		if (typedItem != null)
		{
			typedItem.Parent = this.parent;
		}

		this.items.Insert(index, item);
	}

	public void RemoveAt(int index)
	{
		this.items.RemoveAt(index);
	}

	public bool Remove(IItemContainer item)
	{
		return this.Remove(item);
	}

	public IEnumerator<IItemContainer> GetEnumerator()
	{
		return this.items.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return this.GetEnumerator();
	}
}