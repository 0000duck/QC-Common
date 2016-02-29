using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace QuantumConcepts.Common.Extensions
{
    public static class XmlExtensions
    {
        public static void Rename(this XmlNodeList elements, string newPrefix, string newName, string newNamespace)
        {
            for (int i = elements.Count - 1; i >= 0; i--)
            {
                XmlElement element = (elements[i] as XmlElement);

                if (element != null)
                    element.Rename(newPrefix, newName, newNamespace);
            }
        }

        public static XmlElement Rename(this XmlElement element, string newPrefix, string newName, string newNamespace)
        {
            XmlDocument document = element.OwnerDocument;
            XmlElement newElement = document.CreateElement(newPrefix, newName, newNamespace);
            string namespaceToRename = (element.NamespaceURI == newNamespace ? null : element.NamespaceURI);

            element.Replace(newElement);

            while (element.HasChildNodes)
            {
                XmlNode nodeToRename = element.FirstChild;

                if (nodeToRename is XmlElement && string.Equals(namespaceToRename, nodeToRename.NamespaceURI))
                {
                    XmlElement elementToRename = (XmlElement)newElement.AppendChild(nodeToRename);

                    elementToRename.Rename(newPrefix, elementToRename.LocalName, newNamespace);
                    nodeToRename = elementToRename;
                }
                else
                    newElement.AppendChild(nodeToRename);
            }

            while (!element.Attributes.IsNullOrEmpty())
                newElement.Attributes.Append(element.Attributes[0]);

            return newElement;
        }

        public static void Rename(this XmlAttribute attribute, string newPrefix, string newName, string newNamespace)
        {
            XmlDocument document = attribute.OwnerDocument;
            XmlElement ownerElement = attribute.OwnerElement;
            XmlAttribute newAttribute = document.CreateAttribute(newPrefix, newName, newNamespace);

            newAttribute.Value = attribute.Value;
            ownerElement.Attributes.Remove(attribute);
            ownerElement.Attributes.Append(newAttribute);
        }

        public static void Replace(this XmlElement element, XmlElement newElement)
        {
            if (element.ParentNode is XmlDocument)
            {
                XmlDocument document = element.OwnerDocument;

                element.ParentNode.RemoveChild(element);
                document.AppendChild(newElement);
            }
            else
            {
                element.ParentNode.InsertAfter(newElement, element);
                element.ParentNode.RemoveChild(element);
            }
        }
    }
}
