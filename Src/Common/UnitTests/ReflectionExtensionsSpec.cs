using System;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.Patterning.Common.UnitTests
{
	[TestClass]
	public class ReflectionExtensionsSpec
	{
		internal static readonly IAssertion Assert = new Assertion();

		[TestClass]
		public class GivenAMethodImplementingAnInterface
		{
			[TestMethod]
			public void WhenFindingNullDeclaringMethod_ThenThrowsArgumentNullException()
			{
				Assert.Throws<ArgumentNullException>(() => ReflectionExtensions.FindDeclaringMethod(null));
			}

			[TestMethod]
			public void WhenFindingDeclaringMethod_ThenGetsInterfaceMethod()
			{
				var classDone = Reflector<Foo>.GetMethod(x => x.Done());
				var ifaceDone = Reflector<IFoo>.GetMethod(x => x.Done());

				var found = classDone.FindDeclaringMethod();

				Assert.Same(ifaceDone, found);
			}

			public interface IFoo
			{
				void Done();
			}

			public class Foo : IFoo
			{
				public void Done()
				{
					throw new System.NotImplementedException();
				}
			}
		}

		[TestClass]
		public class GivenAMethodNotImplementingAnInterface
		{
			[TestMethod]
			public void WhenFindingNullDeclaringMethod_ThenThrowsArgumentNullException()
			{
				Assert.Throws<ArgumentNullException>(() => ReflectionExtensions.FindDeclaringMethod(null));
			}

			[TestMethod]
			public void WhenFindingDeclaringMethod_ThenReturnsMethodItself()
			{
				var classDone = Reflector<Foo>.GetMethod(x => x.Done());

				var found = classDone.FindDeclaringMethod();

				Assert.Same(classDone, found);
			}

			public class Foo
			{
				public void Done()
				{
					throw new System.NotImplementedException();
				}
			}
		}

		[TestClass]
		public class GivenAPropertyImplementingAnInterface
		{
			[TestMethod]
			public void WhenFindingDeclaringPropertyGetter_ThenGetsInterfacePropertyGetter()
			{
				var classDone = Reflector<Foo>.GetProperty(x => x.Done).GetGetMethod();
				var ifaceDone = Reflector<IFoo>.GetProperty(x => x.Done).GetGetMethod();

				var found = classDone.FindDeclaringMethod();

				Assert.Same(ifaceDone, found);
			}

			public interface IFoo
			{
				string Done { get; set; }
			}

			public class Foo : IFoo
			{
				public string Done { get; set; }
			}
		}

		[TestClass]
		public class GivenAPropertyNotImplementingAnInterface
		{
			[TestMethod]
			public void WhenFindingDeclaringPropertyGetter_ThenGetsInterfacePropertyGetter()
			{
				var classDone = Reflector<Foo>.GetProperty(x => x.Done).GetGetMethod();

				var found = classDone.FindDeclaringMethod();

				Assert.Same(classDone, found);
			}

			public class Foo
			{
				public string Done { get; set; }
			}
		}
	}
}
