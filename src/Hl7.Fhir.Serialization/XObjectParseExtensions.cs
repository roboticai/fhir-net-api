﻿using Hl7.Fhir.Utility;
using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Hl7.Fhir.Serialization
{
    internal static class XObjectParseExtensions
    {
        public static bool IsResourceName(this XName elementName) =>
            Char.IsUpper(elementName.LocalName, 0) && elementName.Namespace == XmlNs.XFHIR;

        public static bool TryGetContainedResource(this XElement xe, out XElement contained)
        {
            contained = null;

            if (xe.HasElements)
            {
                var candidate = xe.Elements().First();

                if (candidate.Name.IsResourceName())
                {
                    contained = candidate;
                    return true;
                }
            }

            // Not on a resource, no name to be found
            return false;
        }

        public static XObject NextElementOrAttribute(this XObject current)
        {
            var scan = current.NextSibling();

            while (scan != null)
            {
                if (scan.NodeType == XmlNodeType.Element ||
                    (scan.NodeType == XmlNodeType.Attribute && scan is XAttribute attr && !isReservedAttribute(attr)))
                    break;
                scan = scan.NextSibling();
            }

            return scan;

            bool isReservedAttribute(XAttribute attr) => attr.IsNamespaceDeclaration || attr.Name == "value";
        }


        public static string GetValue(this XObject current)
        {
            if (current.AtXhtmlDiv())
                return ((XElement)current).ToString(SaveOptions.DisableFormatting);

            return current is XElement xelem ?
                    xelem.Attribute("value")?.Value : current.Value();
        }
    }
}