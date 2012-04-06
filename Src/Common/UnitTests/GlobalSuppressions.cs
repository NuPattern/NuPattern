// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.
//
// To add a suppression to this file, right-click the message in the 
// Error List, point to "Suppress Message(s)", and click 
// "In Project Suppression File".
// You do not need to add suppressions to this file manually.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1014:MarkAssembliesWithClsCompliant")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "Microsoft.VisualStudio.Patterning.UnitTests.PropertyChangedManagerSpec+Foo.#SubscribeChanged(System.Linq.Expressions.Expression`1<System.Func`2<Microsoft.VisualStudio.Patterning.UnitTests.PropertyChangedManagerSpec+Foo,System.Object>>,System.Action`1<Microsoft.VisualStudio.Patterning.UnitTests.PropertyChangedManagerSpec+Foo>)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.GC.Collect", Scope = "member", Target = "Microsoft.VisualStudio.Patterning.UnitTests.PropertyChangedManagerSpec.#WhenSubscriberIsNotAlive_ThenDoesNotNotifySubscriber()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.GC.Collect", Scope = "member", Target = "Microsoft.VisualStudio.Patterning.UnitTests.PropertyChangedManagerSpec.#WhenSubscriptionIsDisposed_ThenDoesNotNotifySubscriber()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.GC.Collect", Scope = "member", Target = "Microsoft.VisualStudio.Patterning.UnitTests.PropertyChangedManagerSpec.#WhenAddedPropertyChangedHandlerTargetIsNotAlive_ThenDoesNotNotify()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Scope = "member", Target = "Microsoft.VisualStudio.Patterning.Common.UnitTests.ReflectionExtensionsSpec+GivenAMethodNotImplementingAnInterface+Foo.#Done()")]
