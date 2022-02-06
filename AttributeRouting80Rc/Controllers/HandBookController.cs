// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using AttributeRouting80Rc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace AttributeRouting80Rc.Controllers
{
    public class BooksController : ODataController
    {
        private BookStoreContext _db;

        public BooksController(BookStoreContext context)
        {
            _db = context;
            _db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            if (context.Books.Count() == 0)
            {
                foreach (var b in DataSource.GetBooks())
                {
                    context.Books.Add(b);
                    context.Presses.Add(b.Press);
                }
                context.SaveChanges();
            }
        }

        [EnableQuery(PageSize = 1)]
        [HttpGet()]
        //[HttpGet("odata/Books")]
        //[HttpGet("odata/Books/$count")]
        public IActionResult Get()
        {
            return Ok(_db.Books);
        }

        [EnableQuery]
        [HttpGet()]
        //[HttpGet("odata/Books({id})")]
        //[HttpGet("odata/Books/{id}")]
        public IActionResult Get(int id)
        {
            return Ok(_db.Books.FirstOrDefault(c => c.Id == id));
        }

        //[Route("odata/Books({key})")]
        //[HttpPatch]
        //[HttpDelete]
        //public IActionResult OperationBook(int key)
        //{
        //    // the return is just for test.
        //    return Ok(_db.Books.FirstOrDefault(c => c.Id == key));
        //}

        [HttpPost()]
        //[HttpPost("Books/{key}/Default.LookupById()")]
        //[HttpPost("Books/{key}/LookupById()")]
        [EnableQuery(MaxExpansionDepth = 100, MaxAnyAllExpressionDepth = 2)]
        public ActionResult<SingleResult<Book>> LookupById([FromODataUri] int key, ODataActionParameters parameters)
        {
            return Ok(SingleResult.Create(_db.Books.Where(c => c.Id == key)));
        }

        [HttpPost]
        [EnableQuery(MaxExpansionDepth = 100, MaxAnyAllExpressionDepth = 3)]
        public ActionResult<IQueryable<Book>> LookupByIds(ODataActionParameters parameters)
        {
            List<int> IDs = null;
            if (parameters != null && parameters.TryGetValue("iDs", out var IDs_As_Object) && (IDs_As_Object != null))
            {
                IDs = new List<int>(((IEnumerable<int>)IDs_As_Object));
            }

            if (IDs != null)
            {
                return Ok(_db.Books.Where(x => IDs.Any(y => y == x.Id)));
            }
            else
            {
                return Ok(_db.Books);
            }
        }
    }
}
