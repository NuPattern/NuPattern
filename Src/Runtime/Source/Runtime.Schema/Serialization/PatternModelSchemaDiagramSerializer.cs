using System;
using System.Globalization;
using System.Xml;
using Microsoft.VisualStudio.Modeling;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// PatternModel diagram serializer extension class.
    /// </summary>
    public partial class PatternModelSchemaDiagramSerializer
    {
        /// <summary>
        /// Public Write() method that serializes one PatternModelSchemaDiagram instance into XML.
        /// </summary>
        /// <param name="serializationContext">Serialization context.</param>
        /// <param name="element">PatternModelSchemaDiagram instance to be serialized.</param>
        /// <param name="writer">XmlWriter to write serialized data to.</param>
        /// <param name="rootElementSettings">The root element settings if the passed in element is serialized as a root element in the XML. The root element contains additional
        /// information like schema target namespace, version, etc.
        /// This should only be passed for root-level elements. Null should be passed for rest elements (and ideally call the Write() method
        /// without this parameter).</param>
        public override void Write(SerializationContext serializationContext, ModelElement element, XmlWriter writer, RootElementSettings rootElementSettings)
        {
            Guard.NotNull(() => serializationContext, serializationContext);
            Guard.NotNull(() => element, element);
            Guard.NotNull(() => writer, writer);
            Guard.NotNull(() => rootElementSettings, rootElementSettings);

            if (rootElementSettings != null && !string.IsNullOrEmpty(rootElementSettings.SchemaTargetNamespace))
            {
                writer.WriteStartElement(this.XmlTagName, rootElementSettings.SchemaTargetNamespace);
                SerializationUtilities.WriteDomainModelNamespaces(serializationContext.Directory, writer, rootElementSettings.SchemaTargetNamespace);
            }
            else
            {
                writer.WriteStartElement(this.XmlTagName);
            }

            if (rootElementSettings != null && rootElementSettings.Version != null)
            {
                writer.WriteAttributeString("dslVersion", rootElementSettings.Version.ToString(4));
            }

            writer.WriteAttributeString("Id", element.Id.ToString("D", CultureInfo.CurrentCulture));

            this.WritePropertiesAsAttributes(serializationContext, element, writer);

            if (rootElementSettings != null && !serializationContext.Result.Failed)
            {
                PatternModelSerializationHelper.Instance.WriteExtensions(serializationContext, element, writer);
            }

            if (!serializationContext.Result.Failed)
            {
                WriteElements(serializationContext, element, writer);
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// This method creates an instance of PatternModelSchemaDiagram based on the tag currently pointed by the reader. The reader is guaranteed (by the caller)
        /// to be pointed at a serialized instance of PatternModelSchemaDiagram.
        /// </summary>
        /// <param name="serializationContext">Serialization context.</param>
        /// <param name="reader">XmlReader to read serialized data from.</param>
        /// <param name="partition">Partition in which new PatternModelSchemaDiagram instance should be created.</param>
        /// <returns>Created PatternModelSchemaDiagram instance.</returns>
        /// <remarks>
        /// The caller will guarantee that the reader is positioned at open XML tag of the ModelRoot instance being read. This method should
        /// not move the reader; the reader should remain at the same position when this method returns.
        /// </remarks>
        protected override ModelElement CreateInstance(SerializationContext serializationContext, XmlReader reader, Partition partition)
        {
            Guard.NotNull(() => serializationContext, serializationContext);
            Guard.NotNull(() => reader, reader);
            Guard.NotNull(() => partition, partition);

            var typeAttribute = reader.GetAttribute("type");

            if (String.IsNullOrEmpty(typeAttribute))
            {
                return base.CreateInstance(serializationContext, reader, partition);
            }

            var idstring = reader.GetAttribute("Id");

            try
            {
                Guid id;

                if (string.IsNullOrEmpty(idstring))
                {
                    id = Guid.NewGuid();
                    PatternModelSerializationBehaviorSerializationMessages.MissingId(serializationContext, reader, id);
                }
                else
                {
                    id = new Guid(idstring);
                }

                var diagramType = this.GetType().Assembly.GetType(typeAttribute);

                return (ModelElement)Activator.CreateInstance(diagramType, partition, new PropertyAssignment(ElementFactory.IdPropertyAssignment, id));
            }
            catch (ArgumentNullException)
            {
                PatternModelSerializationBehaviorSerializationMessages.InvalidPropertyValue(serializationContext, reader, "Id", typeof(Guid), idstring);
            }
            catch (FormatException)
            {
                PatternModelSerializationBehaviorSerializationMessages.InvalidPropertyValue(serializationContext, reader, "Id", typeof(Guid), idstring);
            }
            catch (OverflowException)
            {
                PatternModelSerializationBehaviorSerializationMessages.InvalidPropertyValue(serializationContext, reader, "Id", typeof(Guid), idstring);
            }

            return null;
        }

        /// <summary>
        /// Write all properties that need to be serialized as XML attributes.
        /// </summary>
        /// <param name="serializationContext">Serialization context.</param>
        /// <param name="element">PatternModelSchemaDiagram instance to be serialized.</param>
        /// <param name="writer">XmlWriter to write serialized data to.</param>
        protected override void WritePropertiesAsAttributes(SerializationContext serializationContext, ModelElement element, XmlWriter writer)
        {
            Guard.NotNull(() => serializationContext, serializationContext);
            Guard.NotNull(() => element, element);
            Guard.NotNull(() => writer, writer);

            writer.WriteAttributeString("type", element.GetType().FullName);
            base.WritePropertiesAsAttributes(serializationContext, element, writer);
        }
    }
}