using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Extensibility;
using NuPattern.Library.Automation;
using NuPattern.Runtime;
using NuPattern.Runtime.Store;

namespace NuPattern.Library.UnitTests
{
    /// <summary>
    /// Test Data for this class:
    /// 
    /// (A Pattern with single View, with mutiple nested Elements: E through W)
    /// All elements, and pattern have a single variable property
    /// All CodeIdentifiers are same as element names
    /// All InstanceNames are same as element names with a numeral appended (i.e. W1, W2, W3).
    /// 
    /// AProduct1
    ///		-> AView1
    ///			-> O1
    ///			-> N1
    ///			-> Q1	
    ///				-> T1
    ///					-> U1
    ///					-> V1
    ///					-> M1
    ///				-> S1
    ///				-> R1
    ///					-> L1
    ///					-> E1
    ///						-> F1
    ///						-> G1
    /// </summary>
    public class ProductElementDictionaryConverterSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public abstract class GivenAnEmptyProduct
        {
            internal Product Product
            {
                get;
                private set;
            }

            internal ProductElementDictionaryConverter Converter
            {
                get;
                private set;
            }

            [TestInitialize]
            public virtual void Initialize()
            {
                this.Converter = new ProductElementDictionaryConverter(PluralizationService.CreateService(new System.Globalization.CultureInfo("en-US")));

                this.Product = CreateProduct();
            }
        }

        [TestClass]
        public abstract class GivenAFullHierarchy : GivenAnEmptyProduct
        {
            internal View View { get; private set; }

            [TestInitialize]
            public override void Initialize()
            {
                base.Initialize();

                CreateProperty(this.Product, this.Product.DefinitionName + "Property1", this.Product.DefinitionName + "Value1");

                // Build the Tree
                this.View = CreateView(this.Product, "AView", v =>
                {
                    CreateElement(v, "O");
                    CreateElement(v, "N");
                    CreateElement(v, "Q", Cardinality.OneToOne, q =>
                    {
                        CreateElement(q, "T", Cardinality.OneToOne, t =>
                        {
                            CreateElement(t, "U");
                            CreateElement(t, "V");
                            CreateElement(t, "M", Cardinality.OneToOne);
                        });
                        CreateElement(q, "S");
                        CreateElement(q, "R", Cardinality.OneToOne, r =>
                        {
                            CreateElement(r, "L");
                            CreateElement(r, "E", Cardinality.OneToOne, e =>
                            {
                                CreateElement(e, "F");
                                CreateElement(e, "G");
                            });
                        });
                    });
                });
            }
        }

        [TestClass]
        public class GivenAProductWithNoElements : GivenAnEmptyProduct
        {
            [TestInitialize]
            public override void Initialize()
            {
                base.Initialize();
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenProductHasNoProperties_ThenReturnsBasicProperties()
            {
                var dictionary = this.Converter.ConvertToDictionary(this.Product);

                AssertBasicProperties(this.Product, dictionary);
                Assert.False(dictionary.Any());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenProductHasOneProperty_ThenReturnDictionary()
            {
                CreateProperty(this.Product, "Property", "Value");

                var dictionary = this.Converter.ConvertToDictionary(this.Product);

                AssertBasicProperties(this.Product, dictionary);
                AssertDictionaryValue(dictionary, "Property", "Value");
                Assert.False(dictionary.Any());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenProductHasTwoProperties_ThenReturnDictionary()
            {
                var property1 = CreateProperty(this.Product, "Property1", "Value1");
                var property2 = CreateProperty(this.Product, "Property2", "Value2");

                var dictionary = this.Converter.ConvertToDictionary(this.Product);

                AssertBasicProperties(this.Product, dictionary);
                AssertDictionaryValue(dictionary, "Property1", "Value1");
                AssertDictionaryValue(dictionary, "Property2", "Value2");
                Assert.False(dictionary.Any());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenProductHasAView_ThenReturnDictionary()
            {
                var view = CreateView(this.Product, "AView");

                var dictionary = this.Converter.ConvertToDictionary(this.Product);

                AssertBasicProperties(this.Product, dictionary);
                AssertPathDownSingletonTree(dictionary, "AView");
                Assert.False(dictionary.Any());
            }
        }

        [TestClass]
        public class GivenAProductWithAnOneToOneElement : GivenAnEmptyProduct
        {
            internal View View
            {
                get;
                private set;
            }

            internal Element Element
            {
                get;
                private set;
            }

            [TestInitialize]
            public override void Initialize()
            {
                base.Initialize();
                CreateProperty(this.Product, this.Product.DefinitionName + "Property1", this.Product.DefinitionName + "Value1");
                this.View = CreateView(this.Product, "AView");
                this.Element = CreateElement(this.View, "N", Cardinality.OneToOne);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenElement_ThenReturnDictionary()
            {
                var dictionary = this.Converter.ConvertToDictionary(this.Element);

                AssertPathUpThroughSingletonTree(dictionary, string.Empty, "N");
                AssertPathUpThroughSingletonTree(dictionary, "Parent", "AView");
                AssertPathUpThroughSingletonTree(dictionary, "Parent.Parent", "AProduct");
                Assert.False(dictionary.Any());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenProduct_ThenReturnDictionary()
            {
                var dictionary = this.Converter.ConvertToDictionary(this.Product);

                AssertPathDownSingletonTree(dictionary, "AProduct");
                AssertPathDownSingletonTree(dictionary, "AView");
                AssertPathDownSingletonTree(dictionary, "AView.N");
                Assert.False(dictionary.Any());
            }
        }

        [TestClass]
        public class GivenAProductWithAnOneToManyElement : GivenAnEmptyProduct
        {
            internal View View
            {
                get;
                private set;
            }

            internal Element Element
            {
                get;
                private set;
            }

            [TestInitialize]
            public override void Initialize()
            {
                base.Initialize();
                CreateProperty(this.Product, this.Product.DefinitionName + "Property1", this.Product.DefinitionName + "Value1");
                this.View = CreateView(this.Product, "AView");
                this.Element = CreateElement(this.View, "N", Cardinality.OneToMany);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenElement_ThenReturnDictionary()
            {
                var dictionary = this.Converter.ConvertToDictionary(this.Element);

                AssertPathUpThroughSingletonTree(dictionary, string.Empty, "N");
                AssertPathUpThroughSingletonTree(dictionary, "Parent", "AView");
                AssertPathUpThroughSingletonTree(dictionary, "Parent.Parent", "AProduct");
                Assert.False(dictionary.Any());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenProduct_ThenReturnDictionary()
            {
                var dictionary = this.Converter.ConvertToDictionary(this.Product);

                AssertPathDownSingletonTree(dictionary, "AProduct");
                AssertPathDownSingletonTree(dictionary, "AView");
                AssertDictionaryValue(dictionary, "AView.N[0].NProperty1", "NValue1");
                AssertDictionaryValue(dictionary, "AView.N[0].InstanceName", "N1");
                AssertDictionaryValue(dictionary, "AView.N[0].DefinitionName", "N");
                Assert.False(dictionary.Any());
            }
        }

        [TestClass]
        public class GivenAProductWithAnZeroToManyElement : GivenAnEmptyProduct
        {
            internal View View
            {
                get;
                private set;
            }

            internal Element Element
            {
                get;
                private set;
            }

            [TestInitialize]
            public override void Initialize()
            {
                base.Initialize();
                CreateProperty(this.Product, this.Product.DefinitionName + "Property1", this.Product.DefinitionName + "Value1");
                this.View = CreateView(this.Product, "AView");
                this.Element = CreateElement(this.View, "N", Cardinality.ZeroToMany);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenElement_ThenReturnsDictionary()
            {
                var dictionary = this.Converter.ConvertToDictionary(this.Element);

                AssertPathUpThroughSingletonTree(dictionary, string.Empty, "N");
                AssertPathUpThroughSingletonTree(dictionary, "Parent", "AView");
                AssertPathUpThroughSingletonTree(dictionary, "Parent.Parent", "AProduct");
                Assert.False(dictionary.Any());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenElementCardinalityZeroToMany_ThenProductReturnDictionary()
            {
                var dictionary = this.Converter.ConvertToDictionary(this.Product);

                AssertPathDownSingletonTree(dictionary, "AProduct");
                AssertPathDownSingletonTree(dictionary, "AView");
                AssertDictionaryValue(dictionary, "AView.N[0].NProperty1", "NValue1");
                AssertDictionaryValue(dictionary, "AView.N[0].InstanceName", "N1");
                AssertDictionaryValue(dictionary, "AView.N[0].DefinitionName", "N");
                Assert.False(dictionary.Any());
            }
        }

        [TestClass]
        public class GivenAProduct : GivenAFullHierarchy
        {
            [TestMethod, TestCategory("Unit")]
            public void WhenProduct_ThenReturnDictionary()
            {
                var dictionary = this.Converter.ConvertToDictionary(this.Product);

                AssertPathDownSingletonTree(dictionary, "AProduct");
                AssertPathDownSingletonTree(dictionary, "AView");
                AssertPathDownSingletonTree(dictionary, "AView.O");
                AssertPathDownSingletonTree(dictionary, "AView.N");
                AssertPathDownSingletonTree(dictionary, "AView.Q");
                AssertPathDownSingletonTree(dictionary, "AView.Q.T");
                AssertPathDownSingletonTree(dictionary, "AView.Q.T.U");
                AssertPathDownSingletonTree(dictionary, "AView.Q.T.V");
                AssertPathDownSingletonTree(dictionary, "AView.Q.T.M");
                AssertPathDownSingletonTree(dictionary, "AView.Q.S");
                AssertPathDownSingletonTree(dictionary, "AView.Q.R");
                AssertPathDownSingletonTree(dictionary, "AView.Q.R.L");
                AssertPathDownSingletonTree(dictionary, "AView.Q.R.E");
                AssertPathDownSingletonTree(dictionary, "AView.Q.R.E.F");
                AssertPathDownSingletonTree(dictionary, "AView.Q.R.E.G");

                Assert.False(dictionary.Any());
            }
        }

        [TestClass]
        public class GivenALeaf : GivenAFullHierarchy
        {
            internal Element Leaf { get; private set; }

            [TestInitialize]
            public override void Initialize()
            {
                base.Initialize();

                this.Leaf = this.Product.View
                    .Elements.Find(q => q.DefinitionName == "Q")
                    .Elements.Find(t => t.DefinitionName == "T")
                    .Elements.Find(u => u.DefinitionName == "U") as Element;

            }

            [TestMethod, TestCategory("Unit")]
            public void WhenLeaf_ThenReturnDictionary()
            {
                var dictionary = this.Converter.ConvertToDictionary(this.Leaf);

                AssertPathUpThroughSingletonTree(dictionary, string.Empty, "U");
                AssertPathUpThroughSingletonTree(dictionary, "Parent", "T");
                AssertPathUpThroughSingletonTree(dictionary, "Parent.V", "V");
                AssertPathUpThroughSingletonTree(dictionary, "Parent.M", "M");
                AssertPathUpThroughSingletonTree(dictionary, "Parent.Parent", "Q");
                AssertPathUpThroughSingletonTree(dictionary, "Parent.Parent.S", "S");
                AssertPathUpThroughSingletonTree(dictionary, "Parent.Parent.R", "R");
                AssertPathUpThroughSingletonTree(dictionary, "Parent.Parent.R.L", "L");
                AssertPathUpThroughSingletonTree(dictionary, "Parent.Parent.R.E", "E");
                AssertPathUpThroughSingletonTree(dictionary, "Parent.Parent.R.E.F", "F");
                AssertPathUpThroughSingletonTree(dictionary, "Parent.Parent.R.E.G", "G");
                AssertPathUpThroughSingletonTree(dictionary, "Parent.Parent.Parent", "AView");
                AssertPathUpThroughSingletonTree(dictionary, "Parent.Parent.Parent.N", "N");
                AssertPathUpThroughSingletonTree(dictionary, "Parent.Parent.Parent.O", "O");
                AssertPathUpThroughSingletonTree(dictionary, "Parent.Parent.Parent.Parent", "AProduct");

                Assert.False(dictionary.Any());
            }
        }

        [TestClass]
        public class GivenABranch : GivenAFullHierarchy
        {
            internal Element Branch { get; private set; }

            [TestInitialize]
            public override void Initialize()
            {
                base.Initialize();

                this.Branch = this.Product.View
                    .Elements.Find(q => q.DefinitionName == "Q")
                    .Elements.Find(t => t.DefinitionName == "R") as Element;
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenBranch_ThenReturnDictionary()
            {
                var dictionary = this.Converter.ConvertToDictionary(this.Branch);

                AssertPathUpThroughSingletonTree(dictionary, string.Empty, "R");
                AssertPathDownSingletonTree(dictionary, "L");
                AssertPathDownSingletonTree(dictionary, "E");
                AssertPathDownSingletonTree(dictionary, "E.G");
                AssertPathDownSingletonTree(dictionary, "E.F");
                AssertPathUpThroughSingletonTree(dictionary, "Parent", "Q");
                AssertPathUpThroughSingletonTree(dictionary, "Parent.S", "S");
                AssertPathUpThroughSingletonTree(dictionary, "Parent.T", "T");
                AssertPathUpThroughSingletonTree(dictionary, "Parent.T.M", "M");
                AssertPathUpThroughSingletonTree(dictionary, "Parent.T.V", "V");
                AssertPathUpThroughSingletonTree(dictionary, "Parent.T.U", "U");
                AssertPathUpThroughSingletonTree(dictionary, "Parent.Parent", "AView");
                AssertPathUpThroughSingletonTree(dictionary, "Parent.Parent.N", "N");
                AssertPathUpThroughSingletonTree(dictionary, "Parent.Parent.O", "O");
                AssertPathUpThroughSingletonTree(dictionary, "Parent.Parent.Parent", "AProduct");

                Assert.False(dictionary.Any());
            }
        }

        #region Helper Methods

        private static Product CreateProduct()
        {
            ProductState productStore = null;
            DslTestStore<ProductStateStoreDomainModel> store = new DslTestStore<ProductStateStoreDomainModel>();
            store.TransactionManager.DoWithinTransaction(() =>
            {
                productStore = store.ElementFactory.CreateElement<ProductState>();
            });

            var product = (Product)productStore.CreateProduct();
            product.InstanceName = "AProduct1";
            product.Info = Mock.Of<IPatternInfo>(i => i.Name == "AProduct" && i.CodeIdentifier == "AProduct");

            return product;
        }

        private static View CreateView(Product product, string viewName)
        {
            var view = (View)product.CreateView();
            view.Info = Mock.Of<IViewInfo>(vi => vi.Name == viewName && vi.CodeIdentifier == viewName);

            product.Store.TransactionManager.DoWithinTransaction(() =>
            {
                product.View = view;
            });

            return view;
        }

        private static View CreateView(Product product, string viewName, Action<View> initializer)
        {
            var view = CreateView(product, viewName);

            initializer(view);

            return view;
        }

        private static Element CreateElement(IElementContainer parent, string elementName, Cardinality cardinality)
        {
            var child = (Element)parent.CreateElement();
            var elementInstanceCount = parent.Elements.Where(e => e.DefinitionName == elementName).Count() + 1;

            child.DefinitionName = elementName;
            child.InstanceName = elementName + elementInstanceCount.ToString();
            child.Info = Mock.Of<IElementInfo>(ei => ei.Name == elementName && ei.CodeIdentifier == elementName && ei.Cardinality == cardinality);

            var property = CreateProperty(child, elementName + "Property1", elementName + "Value1");

            return child;
        }

        private static Element CreateElement(IElementContainer parent, string elementName, Cardinality cardinality = Cardinality.OneToOne, Action<Element> initializer = null)
        {
            var element = CreateElement(parent, elementName, cardinality);

            if (initializer != null)
            {
                initializer(element);
            }

            return element;
        }

        private static Property CreateProperty(IProductElement element, string propertyName, string value)
        {
            var property = (Property)element.CreateProperty();
            property.Info = Mock.Of<IPropertyInfo>(pi => pi.Name == propertyName && pi.CodeIdentifier == propertyName && pi.Type == "System.String");
            property.RawValue = value;

            return property;
        }

        private static void AssertDictionaryValue(IDictionary<string, string> dictionary, string propertyName, string propertyValue)
        {
            Assert.True(dictionary.Keys.Contains(propertyName), string.Format(CultureInfo.CurrentCulture, "{0} key not found in dictionary.", propertyName));
            Assert.Equal(propertyValue, dictionary[propertyName]);

            //Remove item from dictionary
            dictionary.Remove(propertyName);
        }

        private static void AssertPathDownSingletonTree(IDictionary<string, string> dictionary, string descendantPath)
        {
            switch (descendantPath)
            {
                case "AProduct":
                    AssertDictionaryValue(dictionary, "DefinitionName", descendantPath);
                    AssertDictionaryValue(dictionary, "InstanceName", descendantPath + "1");
                    AssertDictionaryValue(dictionary, descendantPath + "Property1", descendantPath + "Value1");
                    break;

                case "AView":
                    AssertDictionaryValue(dictionary, "AView.DefinitionName", "AView");
                    break;

                default:
                    var leaf = descendantPath.Split('.').Last();

                    AssertDictionaryValue(dictionary, descendantPath + ".DefinitionName", leaf);
                    AssertDictionaryValue(dictionary, descendantPath + ".InstanceName", leaf + "1");
                    AssertDictionaryValue(dictionary, descendantPath + "." + leaf + "Property1", leaf + "Value1");
                    break;
            }
        }

        private static void AssertPathUpThroughSingletonTree(IDictionary<string, string> dictionary, string parentPath, string elementName)
        {
            switch (elementName)
            {
                case "AProduct":
                    AssertDictionaryValue(dictionary, parentPath + ".DefinitionName", elementName);
                    AssertDictionaryValue(dictionary, parentPath + ".InstanceName", elementName + "1");
                    AssertDictionaryValue(dictionary, parentPath + "." + elementName + "Property1", elementName + "Value1");
                    break;

                case "AView":
                    AssertDictionaryValue(dictionary, parentPath + ".DefinitionName", "AView");
                    break;

                default:
                    if (String.IsNullOrEmpty(parentPath))
                    {
                        AssertDictionaryValue(dictionary, "DefinitionName", elementName);
                        AssertDictionaryValue(dictionary, "InstanceName", elementName + "1");
                        AssertDictionaryValue(dictionary, elementName + "Property1", elementName + "Value1");
                    }
                    else
                    {
                        AssertDictionaryValue(dictionary, parentPath + ".DefinitionName", elementName);
                        AssertDictionaryValue(dictionary, parentPath + ".InstanceName", elementName + "1");
                        AssertDictionaryValue(dictionary, parentPath + "." + elementName + "Property1", elementName + "Value1");
                    }
                    break;
            }
        }

        private static void AssertBasicProperties(IProductElement element, IDictionary<string, string> dictionary)
        {
            AssertDictionaryValue(dictionary, "InstanceName", element.InstanceName);
            AssertDictionaryValue(dictionary, "DefinitionName", element.DefinitionName);
        }

        #endregion
    }
}