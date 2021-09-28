using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace AWSManagement.API.Models
{
    public class Catalog
    {
       
        public string id { get; set; }
        
        public string name { get; set; }

        //public override bool Equals(object obj)
        //{
        //    Catalog other = obj as Catalog;
        //    if (other == null)
        //    {
        //        return false;
        //    }
        //    return this.id == other.id;
        //}
        public override int GetHashCode()
        {
            return this.id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this.id == ((Catalog)obj).id;
        }
    }
}
