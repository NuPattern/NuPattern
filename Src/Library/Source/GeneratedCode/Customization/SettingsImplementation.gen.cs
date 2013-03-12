﻿
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using NuPattern.Runtime;


namespace NuPattern.Library.Automation
{ 
	/// <summary>
	/// Configures the settings for adding a project or item template to unfold, and execute other automation on this element..
	/// </summary>
	public partial class TemplateSettings : INotifyPropertyChanged
	{ 
		private PropertyChangeManager propertyChanges;
	
		/// <summary>
		/// Gets the manager for property change event subscriptions for this instance 
		///	and any of its derived classes.
		/// </summary>
		protected PropertyChangeManager PropertyChanges
		{
			get
			{
				if (this.propertyChanges == null)
				{
					this.propertyChanges = new PropertyChangeManager(this);
				}
	
				return this.propertyChanges;
			}
		}
	
		/// <summary>
		/// Provides property change subscription.
		/// </summary>
		IDisposable ITemplateSettings.SubscribeChanged(Expression<Func<ITemplateSettings, object>> propertyExpression, Action<ITemplateSettings> callbackAction)
		{
			return this.PropertyChanges.SubscribeChanged(propertyExpression, callbackAction);
		}
	
		/// <summary>
		/// Exposes the property changed event.
		/// </summary>
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
		{
			add { this.PropertyChanges.AddHandler(value); }
			remove { this.PropertyChanges.RemoveHandler(value); }
		}
	}
	
	/// <summary>
	/// Configures the settings for adding a project or item template to unfold, and execute other automation on this element.
	/// </summary>
	[GeneratedCode("NuPattern", "1.2.0.0")]
	public partial class TemplateSettings : ITemplateSettings 
	{ 
		/// <summary>
		/// Gets the owner of this automation settings.
		/// </summary>
		public IPatternElementSchema Owner
		{
			get { return (IPatternElementSchema)((IAutomationSettingsSchema)this.Extends).Parent; }
		}
	
		/// <summary>
		/// Gets the name of this automation settings.
		/// </summary>
		public string Name
		{
			get { return ((INamedElementInfo)this.Extends).Name; }
		}
	
		/// <summary>
		/// Gets the name of this automation settings.
		/// </summary>
		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, "({0})", this.GetType().Name);
		}
	}
}

namespace NuPattern.Library.Automation
{ 
	/// <summary>
	/// Configures the settings for handling an event for executing other automation on this element..
	/// </summary>
	public partial class EventSettings : INotifyPropertyChanged
	{ 
		private PropertyChangeManager propertyChanges;
	
		/// <summary>
		/// Gets the manager for property change event subscriptions for this instance 
		///	and any of its derived classes.
		/// </summary>
		protected PropertyChangeManager PropertyChanges
		{
			get
			{
				if (this.propertyChanges == null)
				{
					this.propertyChanges = new PropertyChangeManager(this);
				}
	
				return this.propertyChanges;
			}
		}
	
		/// <summary>
		/// Provides property change subscription.
		/// </summary>
		IDisposable IEventSettings.SubscribeChanged(Expression<Func<IEventSettings, object>> propertyExpression, Action<IEventSettings> callbackAction)
		{
			return this.PropertyChanges.SubscribeChanged(propertyExpression, callbackAction);
		}
	
		/// <summary>
		/// Exposes the property changed event.
		/// </summary>
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
		{
			add { this.PropertyChanges.AddHandler(value); }
			remove { this.PropertyChanges.RemoveHandler(value); }
		}
	}
	
	/// <summary>
	/// Configures the settings for handling an event for executing other automation on this element.
	/// </summary>
	[GeneratedCode("NuPattern", "1.2.0.0")]
	public partial class EventSettings : IEventSettings 
	{ 
		/// <summary>
		/// Gets the owner of this automation settings.
		/// </summary>
		public IPatternElementSchema Owner
		{
			get { return (IPatternElementSchema)((IAutomationSettingsSchema)this.Extends).Parent; }
		}
	
		/// <summary>
		/// Gets the name of this automation settings.
		/// </summary>
		public string Name
		{
			get { return ((INamedElementInfo)this.Extends).Name; }
		}
	
		/// <summary>
		/// Gets the name of this automation settings.
		/// </summary>
		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, "({0})", this.GetType().Name);
		}
	}
}

namespace NuPattern.Library.Automation
{ 
	/// <summary>
	/// Configures the settings for adding a command that can be executed on this element..
	/// </summary>
	public partial class CommandSettings : INotifyPropertyChanged
	{ 
		private PropertyChangeManager propertyChanges;
	
		/// <summary>
		/// Gets the manager for property change event subscriptions for this instance 
		///	and any of its derived classes.
		/// </summary>
		protected PropertyChangeManager PropertyChanges
		{
			get
			{
				if (this.propertyChanges == null)
				{
					this.propertyChanges = new PropertyChangeManager(this);
				}
	
				return this.propertyChanges;
			}
		}
	
		/// <summary>
		/// Provides property change subscription.
		/// </summary>
		IDisposable ICommandSettings.SubscribeChanged(Expression<Func<ICommandSettings, object>> propertyExpression, Action<ICommandSettings> callbackAction)
		{
			return this.PropertyChanges.SubscribeChanged(propertyExpression, callbackAction);
		}
	
		/// <summary>
		/// Exposes the property changed event.
		/// </summary>
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
		{
			add { this.PropertyChanges.AddHandler(value); }
			remove { this.PropertyChanges.RemoveHandler(value); }
		}
	}
	
	/// <summary>
	/// Configures the settings for adding a command that can be executed on this element.
	/// </summary>
	[GeneratedCode("NuPattern", "1.2.0.0")]
	public partial class CommandSettings : ICommandSettings 
	{ 
		/// <summary>
		/// Gets the owner of this automation settings.
		/// </summary>
		public IPatternElementSchema Owner
		{
			get { return (IPatternElementSchema)((IAutomationSettingsSchema)this.Extends).Parent; }
		}
	
		/// <summary>
		/// Gets the name of this automation settings.
		/// </summary>
		public string Name
		{
			get { return ((INamedElementInfo)this.Extends).Name; }
		}
	
		/// <summary>
		/// Gets the name of this automation settings.
		/// </summary>
		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, "({0})", this.GetType().Name);
		}
	}
}

namespace NuPattern.Library.Automation
{ 
	/// <summary>
	/// Configures the settings for adding a context menu item for executing other automation on this element..
	/// </summary>
	public partial class MenuSettings : INotifyPropertyChanged
	{ 
		private PropertyChangeManager propertyChanges;
	
		/// <summary>
		/// Gets the manager for property change event subscriptions for this instance 
		///	and any of its derived classes.
		/// </summary>
		protected PropertyChangeManager PropertyChanges
		{
			get
			{
				if (this.propertyChanges == null)
				{
					this.propertyChanges = new PropertyChangeManager(this);
				}
	
				return this.propertyChanges;
			}
		}
	
		/// <summary>
		/// Provides property change subscription.
		/// </summary>
		IDisposable IMenuSettings.SubscribeChanged(Expression<Func<IMenuSettings, object>> propertyExpression, Action<IMenuSettings> callbackAction)
		{
			return this.PropertyChanges.SubscribeChanged(propertyExpression, callbackAction);
		}
	
		/// <summary>
		/// Exposes the property changed event.
		/// </summary>
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
		{
			add { this.PropertyChanges.AddHandler(value); }
			remove { this.PropertyChanges.RemoveHandler(value); }
		}
	}
	
	/// <summary>
	/// Configures the settings for adding a context menu item for executing other automation on this element.
	/// </summary>
	[GeneratedCode("NuPattern", "1.2.0.0")]
	public partial class MenuSettings : IMenuSettings 
	{ 
		/// <summary>
		/// Gets the owner of this automation settings.
		/// </summary>
		public IPatternElementSchema Owner
		{
			get { return (IPatternElementSchema)((IAutomationSettingsSchema)this.Extends).Parent; }
		}
	
		/// <summary>
		/// Gets the name of this automation settings.
		/// </summary>
		public string Name
		{
			get { return ((INamedElementInfo)this.Extends).Name; }
		}
	
		/// <summary>
		/// Gets the name of this automation settings.
		/// </summary>
		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, "({0})", this.GetType().Name);
		}
	}
}

namespace NuPattern.Library.Automation
{ 
	/// <summary>
	/// Configures the settings for associating guidance to this element..
	/// </summary>
	public partial class GuidanceExtension : INotifyPropertyChanged
	{ 
		private PropertyChangeManager propertyChanges;
	
		/// <summary>
		/// Gets the manager for property change event subscriptions for this instance 
		///	and any of its derived classes.
		/// </summary>
		protected PropertyChangeManager PropertyChanges
		{
			get
			{
				if (this.propertyChanges == null)
				{
					this.propertyChanges = new PropertyChangeManager(this);
				}
	
				return this.propertyChanges;
			}
		}
	
		/// <summary>
		/// Provides property change subscription.
		/// </summary>
		IDisposable IGuidanceExtension.SubscribeChanged(Expression<Func<IGuidanceExtension, object>> propertyExpression, Action<IGuidanceExtension> callbackAction)
		{
			return this.PropertyChanges.SubscribeChanged(propertyExpression, callbackAction);
		}
	
		/// <summary>
		/// Exposes the property changed event.
		/// </summary>
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
		{
			add { this.PropertyChanges.AddHandler(value); }
			remove { this.PropertyChanges.RemoveHandler(value); }
		}
	}
	
	/// <summary>
	/// Configures the settings for associating guidance to this element.
	/// </summary>
	[GeneratedCode("NuPattern", "1.2.0.0")]
	public partial class GuidanceExtension : IGuidanceExtension 
	{ }
}

namespace NuPattern.Library.Automation
{ 
	/// <summary>
	/// Configures the settings for adding a wizard to gather and initialize data for properties on this element..
	/// </summary>
	public partial class WizardSettings : INotifyPropertyChanged
	{ 
		private PropertyChangeManager propertyChanges;
	
		/// <summary>
		/// Gets the manager for property change event subscriptions for this instance 
		///	and any of its derived classes.
		/// </summary>
		protected PropertyChangeManager PropertyChanges
		{
			get
			{
				if (this.propertyChanges == null)
				{
					this.propertyChanges = new PropertyChangeManager(this);
				}
	
				return this.propertyChanges;
			}
		}
	
		/// <summary>
		/// Provides property change subscription.
		/// </summary>
		IDisposable IWizardSettings.SubscribeChanged(Expression<Func<IWizardSettings, object>> propertyExpression, Action<IWizardSettings> callbackAction)
		{
			return this.PropertyChanges.SubscribeChanged(propertyExpression, callbackAction);
		}
	
		/// <summary>
		/// Exposes the property changed event.
		/// </summary>
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
		{
			add { this.PropertyChanges.AddHandler(value); }
			remove { this.PropertyChanges.RemoveHandler(value); }
		}
	}
	
	/// <summary>
	/// Configures the settings for adding a wizard to gather and initialize data for properties on this element.
	/// </summary>
	[GeneratedCode("NuPattern", "1.2.0.0")]
	public partial class WizardSettings : IWizardSettings 
	{ 
		/// <summary>
		/// Gets the owner of this automation settings.
		/// </summary>
		public IPatternElementSchema Owner
		{
			get { return (IPatternElementSchema)((IAutomationSettingsSchema)this.Extends).Parent; }
		}
	
		/// <summary>
		/// Gets the name of this automation settings.
		/// </summary>
		public string Name
		{
			get { return ((INamedElementInfo)this.Extends).Name; }
		}
	
		/// <summary>
		/// Gets the name of this automation settings.
		/// </summary>
		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, "({0})", this.GetType().Name);
		}
	}
}

namespace NuPattern.Library.Automation
{ 
	/// <summary>
	/// Configures settings for managing associated artifacts to this element..
	/// </summary>
	public partial class ArtifactExtension : INotifyPropertyChanged
	{ 
		private PropertyChangeManager propertyChanges;
	
		/// <summary>
		/// Gets the manager for property change event subscriptions for this instance 
		///	and any of its derived classes.
		/// </summary>
		protected PropertyChangeManager PropertyChanges
		{
			get
			{
				if (this.propertyChanges == null)
				{
					this.propertyChanges = new PropertyChangeManager(this);
				}
	
				return this.propertyChanges;
			}
		}
	
		/// <summary>
		/// Provides property change subscription.
		/// </summary>
		IDisposable IArtifactExtension.SubscribeChanged(Expression<Func<IArtifactExtension, object>> propertyExpression, Action<IArtifactExtension> callbackAction)
		{
			return this.PropertyChanges.SubscribeChanged(propertyExpression, callbackAction);
		}
	
		/// <summary>
		/// Exposes the property changed event.
		/// </summary>
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
		{
			add { this.PropertyChanges.AddHandler(value); }
			remove { this.PropertyChanges.RemoveHandler(value); }
		}
	}
	
	/// <summary>
	/// Configures settings for managing associated artifacts to this element.
	/// </summary>
	[GeneratedCode("NuPattern", "1.2.0.0")]
	public partial class ArtifactExtension : IArtifactExtension 
	{ }
}

namespace NuPattern.Library.Automation
{ 
	/// <summary>
	/// Configures settings for managing validation of this element..
	/// </summary>
	public partial class ValidationExtension : INotifyPropertyChanged
	{ 
		private PropertyChangeManager propertyChanges;
	
		/// <summary>
		/// Gets the manager for property change event subscriptions for this instance 
		///	and any of its derived classes.
		/// </summary>
		protected PropertyChangeManager PropertyChanges
		{
			get
			{
				if (this.propertyChanges == null)
				{
					this.propertyChanges = new PropertyChangeManager(this);
				}
	
				return this.propertyChanges;
			}
		}
	
		/// <summary>
		/// Provides property change subscription.
		/// </summary>
		IDisposable IValidationExtension.SubscribeChanged(Expression<Func<IValidationExtension, object>> propertyExpression, Action<IValidationExtension> callbackAction)
		{
			return this.PropertyChanges.SubscribeChanged(propertyExpression, callbackAction);
		}
	
		/// <summary>
		/// Exposes the property changed event.
		/// </summary>
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
		{
			add { this.PropertyChanges.AddHandler(value); }
			remove { this.PropertyChanges.RemoveHandler(value); }
		}
	}
	
	/// <summary>
	/// Configures settings for managing validation of this element.
	/// </summary>
	[GeneratedCode("NuPattern", "1.2.0.0")]
	public partial class ValidationExtension : IValidationExtension 
	{ }
}

namespace NuPattern.Library.Automation
{ 
	/// <summary>
	/// Configures settings for handling a drag drop operations on this element..
	/// </summary>
	public partial class DragDropSettings : INotifyPropertyChanged
	{ 
		private PropertyChangeManager propertyChanges;
	
		/// <summary>
		/// Gets the manager for property change event subscriptions for this instance 
		///	and any of its derived classes.
		/// </summary>
		protected PropertyChangeManager PropertyChanges
		{
			get
			{
				if (this.propertyChanges == null)
				{
					this.propertyChanges = new PropertyChangeManager(this);
				}
	
				return this.propertyChanges;
			}
		}
	
		/// <summary>
		/// Provides property change subscription.
		/// </summary>
		IDisposable IDragDropSettings.SubscribeChanged(Expression<Func<IDragDropSettings, object>> propertyExpression, Action<IDragDropSettings> callbackAction)
		{
			return this.PropertyChanges.SubscribeChanged(propertyExpression, callbackAction);
		}
	
		/// <summary>
		/// Exposes the property changed event.
		/// </summary>
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
		{
			add { this.PropertyChanges.AddHandler(value); }
			remove { this.PropertyChanges.RemoveHandler(value); }
		}
	}
	
	/// <summary>
	/// Configures settings for handling a drag drop operations on this element.
	/// </summary>
	[GeneratedCode("NuPattern", "1.2.0.0")]
	public partial class DragDropSettings : IDragDropSettings 
	{ 
		/// <summary>
		/// Gets the owner of this automation settings.
		/// </summary>
		public IPatternElementSchema Owner
		{
			get { return (IPatternElementSchema)((IAutomationSettingsSchema)this.Extends).Parent; }
		}
	
		/// <summary>
		/// Gets the name of this automation settings.
		/// </summary>
		public string Name
		{
			get { return ((INamedElementInfo)this.Extends).Name; }
		}
	
		/// <summary>
		/// Gets the name of this automation settings.
		/// </summary>
		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, "({0})", this.GetType().Name);
		}
	}
}
