
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Employees.Models;


namespace Employees.Controllers
{
    public class EmployeesController : ApiController
    {
        private EmployeeEntities db = new EmployeeEntities();
        /** Option to search by Name
         * GET method
         * example URLs 
         * http://localhost:64701/api/Employees?querySearch=steve
         * http://localhost:64701/api/Employees    
         **/

        public async Task<IHttpActionResult> GetEmployee([FromUri]PagingParameterModel pagingparametermodel)
        {
            // Parameter is passed from Query string if it is null then it default Value will be pageNumber:1  
            string SortOrder = pagingparametermodel.sortOrder;

            string ColumnName = "Name"; // can be set as parameter ... pagingparametermodel.columnName;
 
            string QuerySearchName = pagingparametermodel.querySearchName;

            var source = Enumerable.Empty<Employee>().AsQueryable();

                source = db.Employees.AsEnumerable().AsQueryable();
   

            if (!string.IsNullOrEmpty(pagingparametermodel.querySearch))
            {
                var propertyQueryInfo = typeof(Employee).GetProperty(pagingparametermodel.querySearchName);

                source = (source.AsEnumerable().Where(
                    a => propertyQueryInfo.GetValue(a, null).ToString().ToLower().Contains(pagingparametermodel.querySearch.ToLower())

                )).AsQueryable();
            }

            var items = source.ToList();


            if(items.Count==0)
            {

                return NotFound();

            }
           
            return Ok(items);
        }


        /** Add new record if the email address is valid and not found in another recored**/
        // POST: api/Employees
        [ResponseType(typeof(Employee))]
        public IHttpActionResult PostEmployee(Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Check if the email address if valid
            if (!IsValidEmail(employee.EmailAddress))
            {

                return BadRequest("Invalid Email Address");
            }

            var source = Enumerable.Empty<Employee>().AsQueryable();

            source = db.Employees.AsQueryable();

            var propertyQueryInfo = typeof(Employee).GetProperty("EmailAddress");
            source = (source.AsEnumerable().Where(
                    a => propertyQueryInfo.GetValue(a, null).ToString().ToLower().Contains(employee.EmailAddress)

                )).AsQueryable();


            int count = source.Count();
            // If the email record if found in another record
            if (count > 0)
            {
               return BadRequest("Duplicate Email Address");

            }

            db.Employees.Add(employee);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = employee.empid }, employee);
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool EmployeeExists(int id)
        {
            return db.Employees.Count(e => e.empid == id) > 0;
        }


        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}