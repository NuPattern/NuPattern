
namespace Microsoft.VisualStudio.Patterning.Common.UnitTests
{
    public class ObjectValidatorSpec
    {
        //internal static readonly IAssertion Assert = new Assertion();

        //[TestClass]
        //public abstract class GivenAnObjectWithOneInvalidProperty
        //{
        //    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Not appropriate as it has side-effects (newing up the object!).")]
        //    protected abstract object GetObject();
        //    protected abstract string ValidatedPropertyName { get; }

        //    [TestMethod, TestCategory("Unit")]
        //    public void WhenValidated_ThenResultContainsPropertyName()
        //    {
        //        var target = GetObject();

        //        var result = ObjectValidator.Validate(target);

        //        Assert.True(result.Any());
        //        Assert.True(result.Any(r => r.MemberNames.Contains(ValidatedPropertyName)));
        //    }

        //    [TestMethod, TestCategory("Unit")]
        //    public void WhenValidated_ThenThrowsInvalidWithPropertyName()
        //    {
        //        var target = GetObject();

        //        Assert.Throws<InvalidOperationException>(ValidatedPropertyName, () => ObjectValidator.ThrowIfInvalid(target));
        //    }
        //}

        //[TestClass]
        //public class GivenAValidatorWithNoCustomMessage : GivenAnObjectWithOneInvalidProperty
        //{
        //    [TestMethod, TestCategory("Unit")]
        //    public void WhenValidated_ThenResultContainsDefaultMessage()
        //    {
        //        var foo = GetObject();

        //        var result = ObjectValidator.Validate(foo);

        //        Assert.Equal(1, result.Count());
        //        Assert.False(string.IsNullOrEmpty(result.First().ErrorMessage));
        //    }

        //    public class Foo
        //    {
        //        [Required]
        //        public string Value { get; set; }
        //    }

        //    protected override object GetObject()
        //    {
        //        return new Foo();
        //    }

        //    protected override string ValidatedPropertyName
        //    {
        //        get { return Reflector<Foo>.GetPropertyName(x => x.Value); }
        //    }
        //}

        //[TestClass]
        //public class GivenAValidatorWithCustomMessage : GivenAnObjectWithOneInvalidProperty
        //{
        //    private const string CustomMessage = "Hello";

        //    [TestMethod, TestCategory("Unit")]
        //    public void WhenValidated_ThenResultContainsCustomMessage()
        //    {
        //        var foo = GetObject();

        //        var result = ObjectValidator.Validate(foo);

        //        Assert.Equal(1, result.Count());
        //        Assert.True(result.First().ErrorMessage.Contains(CustomMessage));
        //    }

        //    public class Foo
        //    {
        //        [Required(ErrorMessage = CustomMessage)]
        //        public string Value { get; set; }
        //    }

        //    protected override object GetObject()
        //    {
        //        return new Foo();
        //    }

        //    protected override string ValidatedPropertyName
        //    {
        //        get { return Reflector<Foo>.GetPropertyName(x => x.Value); }
        //    }
        //}
    }
}
