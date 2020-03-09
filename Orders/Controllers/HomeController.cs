using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Orders.Models;
using System.Configuration;

namespace Orders.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            String connectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            String sql = "SELECT * FROM Orders";
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand(sql, conn);

            List<Order> model = new List<Order>();
            using (conn)
            {
                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var order = new Order();
                    order.Date = (DateTime)rdr["Date"];
                    order.Number = (int)rdr["Number"];
                    order.Company = (string)rdr["Company"];
                    order.Amount = (double)rdr["Amount"];
                    order.Paid = (bool)rdr["Paid"];

                    model.Add(order);
                }
            }
            conn.Close();
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(List<Order> orders)
        {
            if (ModelState.IsValid)
            {
                UpdateOrder(orders);
                TempData["Success"] = "Successfully Updated at " + DateTime.Now;
            }
            return RedirectToAction("Index");
        }
        private void UpdateOrder(List<Order> orders)
        {
            if (orders == null || orders.Count <= 0) return;
            string connectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("Update", conn); // get Update Stored Procedure
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                System.IO.File.WriteAllText(@"orders.txt", ""); //clear text file
                string[] numbers = new string[100];
                int paidCounter = 0;

                foreach (Order order in orders)
                {
                    if (order.Paid) 
                    {
                        numbers[paidCounter] = order.Number.ToString();
                        paidCounter++;
                    }

                    cmd.Parameters.Clear();
                    conn.Open();
                    SqlParameter paramPaid = new SqlParameter();
                    paramPaid.ParameterName = "@Paid";
                    paramPaid.Value = order.Paid;
                    cmd.Parameters.Add(paramPaid);

                    SqlParameter paramNum = new SqlParameter();
                    paramNum.ParameterName = "@Number";
                    paramNum.Value = order.Number;
                    cmd.Parameters.Add(paramNum);


                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                System.IO.File.WriteAllLines(@"orders.txt", numbers);
            }
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
