using System;
using System.Collections.Generic;
using EnvDTE;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Shell;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace NuPattern.Runtime
{
	/// <summary>
	/// Dsl task validation message
	/// </summary>
	[CLSCompliant(false)]
	public class ProductStoreTaskValidationMessage : ValidationMessage
	{
		private TaskCategory category;

		/// <summary>
		/// Initializes a new instance of the <see cref="ProductStoreTaskValidationMessage"/> class.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="description">The description.</param>
		/// <param name="category">The category.</param>
		/// <param name="code">The code.</param>
		/// <param name="violationType">Type of the violation.</param>
		/// <param name="helpKeyword">The help keyword.</param>
		public ProductStoreTaskValidationMessage(ValidationContext context, string description, TaskCategory category, string code, ViolationType violationType, string helpKeyword)
			: base(context, description, code, violationType)
		{
			this.category = category;
			this.HelpKeyword = helpKeyword;
		}

		/// <summary>
		/// Gets the category.
		/// </summary>
		/// <value>The category.</value>
		public virtual TaskCategory Category
		{
			get
			{
				return this.category;
			}
		}

		/// <summary>
		/// Gets the hierarchy.
		/// </summary>
		/// <value>The hierarchy.</value>
		public virtual IVsHierarchy Hierarchy
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// return the name of the first referenced file, if one exists, otherwise null.
		/// </summary>
		/// <value></value>
		public override string File
		{
			get
			{
				string baseFileName = base.File;

				return baseFileName;
			}
		}

		/// <summary>
		/// Gets the type of the error message.
		/// </summary>
		/// <value>The type of the error message.</value>
		public virtual TaskErrorCategory ErrorMessageType
		{
			get
			{
				switch (this.Type)
				{
					case ViolationType.Error:
						return TaskErrorCategory.Error;
					case ViolationType.Warning:
						return TaskErrorCategory.Warning;
					case ViolationType.Message:
						return TaskErrorCategory.Message;
				}

				return TaskErrorCategory.Error;
			}
		}

		/// <summary>
		/// Determines whether the specified task item is match.
		/// </summary>
		/// <param name="taskItem">The task item.</param>
		/// <returns>
		/// 	<c>true</c> if the specified task item is match; otherwise, <c>false</c>.
		/// </returns>
		public bool IsMatch(TaskItem taskItem)
		{
			Guard.NotNull(() => taskItem, taskItem);

			return taskItem.Description == this.Description;
		}

		/// <summary>
		/// Configures the specified task.
		/// </summary>
		/// <param name="task">The task.</param>
		public virtual void Configure(ErrorTask task)
		{
			Guard.NotNull(() => task, task);

			task.Text = this.Description;
			task.Category = this.Category;
			task.ErrorCategory = this.ErrorMessageType;
			task.Document = this.File;
			task.Line = this.Line;
			task.Column = this.Column;
			task.HierarchyItem = this.Hierarchy;
			task.Priority = TaskPriority.Normal;
			task.HelpKeyword = this.HelpKeyword;
			task.Navigate += new EventHandler(OnNavigate);
			task.Deleted += new EventHandler(OnDeleted);
		}

		private void OnNavigate(object sender, EventArgs e)
		{
			var vsContext = this.Context as ProductStoreValidationContext;

			if (vsContext != null)
			{
				var task = sender as ProductStoreValidationTask;

				if (task != null)
				{
					vsContext.OnNavigateToTask(task);
				}
			}
		}

		private void OnDeleted(object sender, EventArgs e)
		{
			var vsContext = this.Context as ProductStoreValidationContext;

			if (vsContext != null)
			{
				var task = sender as ProductStoreValidationTask;

				if (task != null)
				{
					vsContext.OnDeleteTask(task);
				}
			}
		}
	}

	/// <summary>
	/// Dsl validation task
	/// </summary>
	[CLSCompliant(false)]
	public class ProductStoreValidationTask : ErrorTask
	{
		/// <summary>
		/// Gets or sets the message.
		/// </summary>
		/// <value>The message.</value>
		public virtual ProductStoreTaskValidationMessage Message { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ProductStoreValidationTask"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Taken from DSL code")]
		public ProductStoreValidationTask(ProductStoreTaskValidationMessage message)
		{
			Guard.NotNull(() => message, message);

			this.Message = message;
			this.Message.Configure(this);

			base.CanDelete = true;
			base.IsPriorityEditable = false;
			base.IsTextEditable = false;
			base.IsCheckedEditable = false;
			base.Checked = false;
		}

		/// <summary>
		/// Determines whether the specified task is match.
		/// </summary>
		/// <param name="task">The task.</param>
		/// <returns>
		/// 	<c>true</c> if the specified task is match; otherwise, <c>false</c>.
		/// </returns>
		public bool IsMatch(ProductStoreValidationTask task)
		{
			return ((task != null) && task.Message.Equals(this.Message));
		}
	}

	/// <summary>
	/// Dsl validation observer
	/// </summary>
	[CLSCompliant(false)]
	public class ProductStoreValidationObserver : ErrorListObserver
	{
		IServiceProvider serviceProvider;
		private ValidationTaskProvider taskProvider;

		/// <summary>
		/// Initializes a new instance of the <see cref="ProductStoreValidationObserver"/> class.
		/// </summary>
		/// <param name="sp">The sp.</param>
		public ProductStoreValidationObserver(IServiceProvider sp)
			: base(sp)
		{
			this.serviceProvider = sp;
		}

		/// <summary>
		/// Report added messages to the task list.
		/// </summary>
		/// <param name="addedMessage"></param>
		/// <remarks>
		/// methods are called in this order:
		/// 1. OnValidationMessagesChanging
		/// 2. OnValidationMessageRemoved - called once for each message removed.
		/// 3. OnValidationMessageAdded - called once for each message added.
		/// 4. OnValidationMessagesChangedSummary
		/// </remarks>
		protected override void OnValidationMessageAdded(ValidationMessage addedMessage)
		{
			var task = new ProductStoreValidationTask((ProductStoreTaskValidationMessage)addedMessage);
			this.TaskProvider.Tasks.Add(task);
		}

		/// <summary>
		/// Report removed messages to the task list.
		/// </summary>
		/// <param name="removedMessage"></param>
		/// <remarks>
		/// methods are called in this order:
		/// 1. OnValidationMessagesChanging
		/// 2. OnValidationMessageRemoved - called once for each message removed.
		/// 3. OnValidationMessageAdded - called once for each message added.
		/// 4. OnValidationMessagesChangedSummary
		/// </remarks>
		protected override void OnValidationMessageRemoved(ValidationMessage removedMessage)
		{
			if (this.TaskProvider.Tasks.Count != 0)
			{
				var task = new ProductStoreValidationTask((ProductStoreTaskValidationMessage)removedMessage);

				foreach (Task tk in this.TaskProvider.Tasks)
				{
					var taskToRemove = tk as ProductStoreValidationTask;

					if ((taskToRemove != null) && taskToRemove.IsMatch(task))
					{
						this.TaskProvider.Tasks.Remove(taskToRemove);
						break;
					}
				}
			}
		}

		/// <summary>
		/// provides access to the Task List.
		/// </summary>
		/// <value></value>
		public new ValidationTaskProvider TaskProvider
		{
			get
			{
				if (this.taskProvider == null)
				{
					this.taskProvider = new ValidationTaskProvider(this.serviceProvider);
				}
				return this.taskProvider;
			}
		}
	}

	/// <summary>
	/// Dsl validation context
	/// </summary>
	[CLSCompliant(false)]
	public class ProductStoreValidationContext : VsValidationContext
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ProductStoreValidationContext"/> class.
		/// </summary>
		/// <param name="customCategories">The custom categories.</param>
		/// <param name="subjects">The subjects.</param>
		/// <param name="serviceProvider">The service provider.</param>
		public ProductStoreValidationContext(string[] customCategories, IEnumerable<ModelElement> subjects, IServiceProvider serviceProvider)
			: base(customCategories, subjects, serviceProvider)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ProductStoreValidationContext"/> class.
		/// </summary>
		/// <param name="categories">The categories.</param>
		/// <param name="subjects">The subjects.</param>
		/// <param name="serviceProvider">The service provider.</param>
		public ProductStoreValidationContext(ValidationCategories categories, IEnumerable<ModelElement> subjects, IServiceProvider serviceProvider)
			: base(categories, subjects, serviceProvider)
		{
		}

		/// <summary>
		/// Overrideable method to allow the derived class to create messages.
		/// </summary>
		/// <param name="description"></param>
		/// <param name="code"></param>
		/// <param name="violationType"></param>
		/// <param name="elements"></param>
		/// <returns></returns>
		protected override ValidationMessage ConstructValidationMessage(string description, string code, ViolationType violationType, params ModelElement[] elements)
		{
			var message = new ProductStoreTaskValidationMessage(this, description, TaskCategory.BuildCompile, code, violationType, null);
			message.UpdateReferencedModelElements(elements);

			return message;
		}

		/// <summary>
		/// Override to supply one or more diagram navigation commands for the referenced elements.
		/// </summary>
		/// <param name="referencedModelElements"></param>
		/// <returns></returns>
		/// <value>An IList of DynamicMenuCommands</value>
		protected override IList<TaskMenuCommand> GetDiagramNavigationCommands(IList<ModelElement> referencedModelElements)
		{
			//TODO: override for PatternManager node selection
			return new List<TaskMenuCommand>();
		}

		/// <summary>
		/// Override to implement OnDelete behavior for the supplied validation task list item.
		/// </summary>
		/// <param name="task"></param>
		public virtual void OnDeleteTask(ProductStoreValidationTask task)
		{
		}

		/// <summary>
		/// Override to implement OnNavigate behavior for the supplied validation task list item.
		/// The default implementation calls DoCommand on the first item in DiagramNavigateCommands,
		/// and executes the ExplorerNavigateCommand, if available.
		/// </summary>
		/// <param name="task">Task List item</param>
		public virtual void OnNavigateToTask(ProductStoreValidationTask task)
		{
			if (task != null)
			{
				var message = task.Message as ProductStoreTaskValidationMessage;

				var list = (message != null) ? this.GetNavigationCommands(message) : null;

				if ((list != null) && (list.Count > 0))
				{
					int num = 0;
					int priority = list[0].Priority;
					for (int i = 1; i < list.Count; i++)
					{
						if (list[i].Priority < priority)
						{
							num = i;
							priority = list[i].Priority;
						}
					}
					list[num].DoCommand();
				}
			}
		}
	}

	/// <summary>
	/// Dsl validation controller
	/// </summary>
	public class ProductStoreValidationController : VsValidationController
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ProductStoreValidationController"/> class.
		/// </summary>
		public ProductStoreValidationController(IServiceProvider serviceProvider)
			: base(serviceProvider)
		{
		}

		/// <summary>
		/// Provide a context class for the validation
		/// </summary>
		/// <param name="subjects"></param>
		/// <param name="customCategories">A list of custom specified string. This allows the validation method with the given string to be validated.</param>
		/// <returns></returns>
		protected override ValidationContext CreateValidationContext(IEnumerable<ModelElement> subjects, string[] customCategories)
		{
			Guard.NotNull(() => customCategories, customCategories);

			return new ProductStoreValidationContext(customCategories, subjects, this.ServiceProvider);
		}

		/// <summary>
		/// Provide a context class for the validation
		/// </summary>
		/// <param name="subjects"></param>
		/// <param name="categories"></param>
		/// <returns></returns>
		protected override ValidationContext CreateValidationContext(IEnumerable<ModelElement> subjects, ValidationCategories categories)
		{
			return new ProductStoreValidationContext(categories, subjects, this.ServiceProvider);
		}
	}
}
