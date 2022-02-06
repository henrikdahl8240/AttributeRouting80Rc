// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.

using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace AttributeRouting80Rc.Models
{
    public static class EdmModelBuilder
    {
        public static IEdmModel BuildBookModel()
        {
            ODataConventionModelBuilder builder = new();
            builder.EntitySet<Book>("Books");
            builder.EntitySet<Press>("Presses");

            ConfigBook(builder);

            return builder.GetEdmModel();
        }

        private static void ConfigBook(ODataConventionModelBuilder builder)
        {
            builder.EntityType<Book>().Expand(1);

            var action_Book_LookupById = builder.EntityType<Book>()
                .Action("LookupById").ReturnsEntityViaEntitySetPath<Book>("bindingParameter");

            var action_Book_LookupByIds = builder.EntityType<Book>().Collection
                .Action("LookupByIds").ReturnsCollectionViaEntitySetPath<Book>("bindingParameter");
            action_Book_LookupByIds.CollectionParameter<int>("iDs").Optional();
        }
    }
}
