using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.Patterning.Runtime.UnitTests.Store
{
	public class StoreEventBufferingScopeSpec
	{
		internal static readonly IAssertion Assert = new Assertion();

		[TestClass]
		public class GivenABufferingScope
		{
			protected StoreEventBufferingScope Scope { get; private set; }

			[TestInitialize]
			public virtual void Initialize()
			{
				this.Scope = new StoreEventBufferingScope();
			}

			[TestCleanup]
			public virtual void Cleanup()
			{
				this.Scope.Dispose();
			}

			[TestMethod, TestCategory("Unit")]
			public void ThenIsActiveIsTrue()
			{
				Assert.True(StoreEventBufferingScope.IsActive);
			}

			[TestMethod, TestCategory("Unit")]
			public void ThenCurrentIsSame()
			{
				Assert.Same(this.Scope, StoreEventBufferingScope.Current);
			}
		}

		[TestClass]
		public class GivenABufferedEvent : GivenABufferingScope
		{
			private bool eventRaised;

			[TestInitialize]
			public override void Initialize()
			{
				base.Initialize();

				this.Scope.AddEvent(() => eventRaised = true);
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenCompleteCalled_ThenEventIsNotRaisedUntilDisposed()
			{
				this.Scope.Complete();

				Assert.False(eventRaised);
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenDisposedAfterCompleteCalled_ThenEventIsRaised()
			{
				this.Scope.Complete();
				this.Scope.Dispose();

				Assert.True(eventRaised);
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenDisposedCalledWithoutComplete_ThenEventIsNotRaised()
			{
				this.Scope.Dispose();

				Assert.False(eventRaised);
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenDisposedAfterCancelCalled_ThenEventIsNotRaised()
			{
				this.Scope.Cancel();
				this.Scope.Dispose();

				Assert.False(eventRaised);
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenDisposedAfterCompleteThenCancelCalled_ThenEventIsNotRaised()
			{
				this.Scope.Complete();
				this.Scope.Cancel();
				this.Scope.Dispose();

				Assert.False(eventRaised);
			}
		}

		[TestClass]
		public class GivenANestedBufferingScope
		{
			protected StoreEventBufferingScope RootScope { get; private set; }
			protected StoreEventBufferingScope NestedScope { get; private set; }

			private List<string> events = new List<string>();

			[TestInitialize]
			public virtual void Initialize()
			{
				this.RootScope = new StoreEventBufferingScope();
				this.RootScope.AddEvent(() => events.Add("root"));

				this.NestedScope = new StoreEventBufferingScope();
				this.NestedScope.AddEvent(() => events.Add("nested"));
			}

			[TestCleanup]
			public virtual void Cleanup()
			{
				this.NestedScope.Dispose();
				this.RootScope.Dispose();
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenNestedCompleted_ThenNoEventsAreRaised()
			{
				this.NestedScope.Complete();
				this.NestedScope.Dispose();

				Assert.True(this.events.Count == 0, "No events should be fired unless the topmost scope is disposed.");
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenRootScopeCompleted_ThenRaisesRootEventsButNotNestedUncompletedEvents()
			{
				this.RootScope.Complete();
				this.RootScope.Dispose();

				Assert.Equal(1, this.events.Count);
				Assert.Equal("root", this.events[0]);
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenRootScopeCompletedAfterNestedCompletedAndDisposed_ThenRaisesAllEventsInOrder()
			{
				this.NestedScope.Complete();
				this.NestedScope.Dispose();
				this.RootScope.Complete();
				this.RootScope.Dispose();

				Assert.Equal(2, this.events.Count);
				Assert.Equal("root", this.events[0]);
				Assert.Equal("nested", this.events[1]);
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenRootScopeCompletedAfterNestedCanceledAndDisposed_ThenDoesNotRaiseAnyEvents()
			{
				this.NestedScope.Cancel();
				this.NestedScope.Dispose();
				this.RootScope.Complete();
				this.RootScope.Dispose();

				Assert.Equal(0, this.events.Count);
			}
		}
	}
}
