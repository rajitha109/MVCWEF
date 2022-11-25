using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVCWEF.Models;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Xml.Linq;

namespace MVCWEF.Controllers
{
    public class ProductController : Controller
    {
        //Connection String    
        string connectionstring = @"Data Source=DESKTOP-6U7CQKC;Initial Catalog=MvcDemoDb;Integrated Security=True";

        // GET: ProductController       

        [HttpGet]        
        public ActionResult Index()
        {
            //load data
            var productsList = new List<ProductModel>();
            using(SqlConnection sqlcon =new SqlConnection(connectionstring))
            {
                sqlcon.Open();                
                StringBuilder retVal = new StringBuilder();
                SqlCommand cmd = new SqlCommand("getAllProduct", sqlcon);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter sd = new SqlDataAdapter(cmd);
                using (var xmlReader = cmd.ExecuteXmlReader())
                {
                    while (xmlReader.Read())
                    {
                        string s = xmlReader.ReadOuterXml();
                        //Console.WriteLine(s);
                        retVal.Append(s);
                    }
                }
                var data = retVal.ToString();                
                var document = XDocument.Parse(data);

                //var products = document.Elements("Products").Elements();
                var products = document.Elements("Products").Elements("Product");
                //Console.WriteLine(products);
                foreach (var product in products)
                {
                   Console.WriteLine(product);

                    Console.WriteLine("Hello");
                }
                Console.WriteLine("-------");
                productsList = products.Select((s) =>
                new ProductModel()
                {
                    ProductID = (s.Attribute("ProductID") != null) ? int.Parse(s.Attribute("ProductID").Value) : 0,
                    ProductName = s.Attribute("ProductName").Value.ToString(),
                    Price=decimal.Parse(s.Attribute("Price").Value),
                    Count=int.Parse(s.Attribute("Counter").Value)
                }).ToList();


                sqlcon.Close();
            }
            return View(productsList);
           
        }

        // GET: ProductController/Details/5


        public ActionResult AllDetails()
        {
            //load data
            var productsList = new List<ProductModel>();
            var sellersList = new List<SellerModel>();
            using (SqlConnection sqlcon = new SqlConnection(connectionstring))
            {
                sqlcon.Open();
                StringBuilder retVal = new StringBuilder();
                SqlCommand cmd = new SqlCommand("getAllDetails", sqlcon);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter sd = new SqlDataAdapter(cmd);
                using (var xmlReader = cmd.ExecuteXmlReader())
                {
                    while (xmlReader.Read())
                    {
                        string s = xmlReader.ReadOuterXml();
                        //Console.WriteLine(s);
                        retVal.Append(s);
                    }
                }
                var data = retVal.ToString();
                var document = XDocument.Parse(data);
                

                //var products = document.Elements("Products").Elements();
                var products = document.Elements("ExportList").Elements("byProduct").Elements("Product") ;

                //Console.WriteLine(products);
                /*foreach (var product in products)
                {
                    Console.WriteLine(product);

                    Console.WriteLine("Hello");
                }*/
                Console.WriteLine("-------");
                productsList = products.Select((s) =>
                new ProductModel()
                {
                    ProductID = (s.Attribute("ProductID") != null) ? int.Parse(s.Attribute("ProductID").Value) : 0,
                    ProductName = s.Attribute("ProductName").Value.ToString(),
                    Price = decimal.Parse(s.Attribute("Price").Value),
                    Count = int.Parse(s.Attribute("Counter").Value)
                }).ToList();

                var sellers = document.Elements("ExportList").Elements("bySeller").Elements("Seller");
                foreach(var seller in sellers)
                {
                   // Console.WriteLine(seller);
                }
                sellersList = sellers.Select((s) =>
                new SellerModel()
                {
                    Id = (s.Attribute("Id") != null) ? int.Parse(s.Attribute("Id").Value) : 0,
                    Name = s.Attribute("Name").Value.ToString(),
                    
                }).ToList();

                

                var productViewList = new ProductViewModel
                {
                    ProductList = productsList,
                    SellerList = sellersList

                };
                




                sqlcon.Close();
                return View(productViewList);
            }
            


        }
        public ActionResult Details(int id)
        {

            return View();
        }

        // GET: ProductController/Create
        public ActionResult Create()
        {
            return View(new ProductModel());
        }

        // POST: ProductController/Create
        [HttpPost]       
        public ActionResult Create(ProductModel productModel)
        {
            using (SqlConnection sqlcon = new SqlConnection(connectionstring))
            {
                sqlcon.Open();
                string query = "INSERT INTO Product VALUES(@ProductName,@Price,@Counter)";
                SqlCommand sqlCmd = new SqlCommand(query, sqlcon);
                sqlCmd.Parameters.AddWithValue("@ProductName", productModel.ProductName);
                sqlCmd.Parameters.AddWithValue("@Price", productModel.Price);
                sqlCmd.Parameters.AddWithValue("@Counter", productModel.Count);
                sqlCmd.ExecuteNonQuery();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: ProductController/Edit/5
        public ActionResult Edit(int id)
        {
            ProductModel productModel = new ProductModel();
            DataTable dtblproduct=new DataTable();
            using(SqlConnection sqlcon = new SqlConnection(connectionstring))
            {
                sqlcon.Open();
                string query = "SELECT * FROM Product WHERE ProductID = @ProductID";
                SqlDataAdapter sda = new SqlDataAdapter(query, sqlcon);
                sda.SelectCommand.Parameters.AddWithValue("@ProductID", id);
                sda.Fill(dtblproduct);

            }
            if(dtblproduct.Rows.Count == 1)
            {
                productModel.ProductID = Convert.ToInt32(dtblproduct.Rows[0][0].ToString());
                productModel.ProductName = dtblproduct.Rows[0][1].ToString();
                productModel.Price = Convert.ToDecimal(dtblproduct.Rows[0][2].ToString());
                productModel.Count = Convert.ToInt32(dtblproduct.Rows[0][3].ToString());
                Console.WriteLine(productModel.Count);
                return View(productModel);

            }
            else
                return RedirectToAction("Index");

        }

        // POST: ProductController/Edit/5
        [HttpPost]
        public ActionResult Edit(ProductModel productModel)
        {
            using (SqlConnection sqlcon = new SqlConnection(connectionstring))
            {
                sqlcon.Open();
                string query = "UPDATE Product SET ProductName=@ProductName,Price=@Price,Counter=@Count WHERE ProductId=@ProductID";
                SqlCommand sqlCmd = new SqlCommand(query, sqlcon);
                sqlCmd.Parameters.AddWithValue("@ProductID", productModel.ProductID);
                sqlCmd.Parameters.AddWithValue("@ProductName", productModel.ProductName);
                sqlCmd.Parameters.AddWithValue("@Price", productModel.Price);
                sqlCmd.Parameters.AddWithValue("@Counter", productModel.Count);
                sqlCmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

        // GET: ProductController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProductController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
