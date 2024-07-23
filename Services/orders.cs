using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace MyCommonStructure.Services
{
    public class orders
    {
        dbServices ds = new dbServices();
        public async Task<responseData> CreateOrder(requestData rData)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                if (!rData.addInfo.ContainsKey("UserId") || !rData.addInfo.ContainsKey("CarId"))
                {
                    resData.rData["rCode"] = 4;
                    resData.rData["rMessage"] = "Invalid input parameters!";
                    return resData;
                }

                MySqlParameter[] checkParams = new MySqlParameter[]
                {
                    new MySqlParameter("@UserId", rData.addInfo["UserId"]),
                    new MySqlParameter("@CarId", rData.addInfo["CarId"])
                };

                var checkQuery = @"SELECT COUNT(*) FROM pc_student.CarsHeaven_Order WHERE UserId=@UserId AND CarId = @CarId;";
                var dbCheckData = await ds.ExecuteSQLAsync(checkQuery, checkParams);
                if (dbCheckData.Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Car with this Id already exists in Order!";
                }
                else
                {
                    MySqlParameter[] insertParams = new MySqlParameter[]
                    {
                        new MySqlParameter("@UserId", rData.addInfo["UserId"]),
                        new MySqlParameter("@CarId", rData.addInfo["CarId"]),
                    };

                    var insertQuery = @"INSERT INTO pc_student.CarsHeaven_Order (UserId, CarId) VALUES (@UserId, @CarId);";
                    var rowsAffected = await ds.ExecuteSQLAsync(insertQuery, insertParams);
                    if (rowsAffected.Count() == 0)
                    {
                        resData.eventID = rData.eventID;
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "Car added to Order successfully.";
                    }
                    else
                    {
                        resData.rData["rCode"] = 3;
                        resData.rData["rMessage"] = "Failed to add car into Order!";
                    }
                }
            }
            catch (Exception ex)
            {
                resData.rStatus = 402;
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = $"Error: {ex.Message}";
            }
            return resData;
        }

        public async Task<responseData> EditOrder(requestData rData)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                MySqlParameter[] checkParams = new MySqlParameter[]
                {
                    new MySqlParameter("@OrderId", rData.addInfo["OrderId"]),
                    new MySqlParameter("@UserId", rData.addInfo["UserId"]),
                    new MySqlParameter("@CarId", rData.addInfo["CarId"]),
                };

                var query = @"SELECT * FROM pc_student.CarsHeaven_Order WHERE OrderId=@OrderId AND UserId=@UserId AND CarId=@CarId";
                var dbData = ds.ExecuteSQLName(query, checkParams);
                if (dbData[0].Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Car not found in Order!";
                }
                else
                {
                    MySqlParameter[] updateParams = new MySqlParameter[]
                   {
                        new MySqlParameter("@OrderId", rData.addInfo["OrderId"]),
                        new MySqlParameter("@UserId", rData.addInfo["UserId"]),
                        new MySqlParameter("@CarId", rData.addInfo["CarId"]),
                   };
                    var updatequery = @"UPDATE pc_student.CarsHeaven_Order SET CarId = @CarId WHERE UserId = @UserId AND OrderId = @OrderId;";
                    var updatedata = ds.ExecuteInsertAndGetLastId(updatequery, updateParams);
                    if (updatedata != 0)
                    {
                        resData.rData["rCode"] = 3;
                        resData.rData["rMessage"] = "Some error occured, failed to update Order!";
                    }
                    else
                    {
                        resData.eventID = rData.eventID;
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "Order updated successfully.";
                    }
                }
            }
            catch (Exception ex)
            {
                resData.rStatus = 402;
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = $"Error: {ex.Message}";
            }
            return resData;
        }

        public async Task<responseData> DeleteOrder(requestData rData)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                string OrderId = rData.addInfo["OrderId"].ToString();
                string UserId = rData.addInfo["UserId"].ToString();
                string CarId = rData.addInfo["CarId"].ToString();

                MySqlParameter[] para = new MySqlParameter[]
                {
                    new MySqlParameter("@OrderId", OrderId),
                    new MySqlParameter("@UserId", UserId),
                    new MySqlParameter("@CarId", CarId)
                };

                var query = @"SELECT * FROM pc_student.CarsHeaven_Order WHERE OrderId = @OrderId AND UserId=@UserId AND CarId = @CarId;";
                var dbData = ds.ExecuteSQLName(query, para);
                if (dbData.Count == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Car not found in the Order!";
                }
                else
                {
                    para = new MySqlParameter[]
                    {
                        new MySqlParameter("@OrderId", OrderId),
                        new MySqlParameter("@UserId", UserId),
                        new MySqlParameter("@CarId", CarId)
                    };

                    var deleteSql = @"DELETE FROM pc_student.CarsHeaven_Order WHERE OrderId = @OrderId AND UserId = @UserId AND CarId = @CarId;";
                    int rowsAffected = ds.ExecuteInsertAndGetLastId(deleteSql, para);
                    if (rowsAffected > 0)
                    {
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "Car removed from Order successfully.";
                    }
                    else
                    {
                        resData.rData["rCode"] = 2;
                        resData.rData["rMessage"] = "Failed to remove car from Order!";
                    }
                }
            }
            catch (Exception ex)
            {
                resData.rStatus = 402;
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = $"Error: {ex.Message}";
            }
            return resData;
        }

        public async Task<responseData> GetOrderById(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            resData.eventID = req.eventID;
            resData.rData["rMessage"] = "Car found successfully!";
            try
            {
                string OrderId = req.addInfo["OrderId"].ToString();
                string UserId = req.addInfo["UserId"].ToString();
                string CarId = req.addInfo["CarId"].ToString();

                MySqlParameter[] myParams = new MySqlParameter[]
                {
                    new MySqlParameter("@OrderId", req.addInfo["OrderId"]),
                    new MySqlParameter("@UserId", req.addInfo["UserId"]),
                    new MySqlParameter("@CarId", req.addInfo["CarId"])
                };

                string getsql = $"SELECT * FROM pc_student.CarsHeaven_Order " +
                             "WHERE OrderId = @OrderId AND UserId = @UserId AND CarId = @CarId;";
                var Orderdata = ds.ExecuteSQLName(getsql, myParams);
                if (Orderdata == null || Orderdata.Count == 0 || Orderdata[0].Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Car not found!";
                }
                else
                {
                    var OrderData = Orderdata[0][0];
                    resData.rData["OrderId"] = OrderData["OrderId"];
                    resData.rData["UserId"] = OrderData["UserId"];
                    resData.rData["CarId"] = OrderData["CarId"];
                    resData.rData["DateAdded"] = OrderData["DateAdded"];
                }
            }
            catch (Exception ex)
            {
                resData.rStatus = 402;
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = ex + "Enter correct Order details!";
            }
            return resData;
        }

        public async Task<responseData> GetAllOrders(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            resData.eventID = req.eventID;
            try
            {
                var query = @"SELECT * FROM pc_student.CarsHeaven_Order ORDER BY OrderId ASC";
                var dbData = ds.executeSQL(query, null);
                if (dbData == null)
                {
                    resData.rData["rMessage"] = "Some error occurred, can't get Order cars!";
                    resData.rStatus = 1;
                    return resData;
                }

                List<object> OrdersList = new List<object>();
                foreach (var rowSet in dbData)
                {
                    if (rowSet != null)
                    {
                        foreach (var row in rowSet)
                        {
                            if (row != null)
                            {
                                List<string> rowData = new List<string>();

                                foreach (var column in row)
                                {
                                    if (column != null)
                                    {
                                        rowData.Add(column.ToString());
                                    }
                                }
                                var Order = new
                                {
                                    OrderId = rowData.ElementAtOrDefault(0),
                                    UserId = rowData.ElementAtOrDefault(1),
                                    CarId = rowData.ElementAtOrDefault(2),
                                    Quantity = rowData.ElementAtOrDefault(3),
                                    Price = rowData.ElementAtOrDefault(4),
                                    TotalPrice = rowData.ElementAtOrDefault(5)
                                };
                                OrdersList.Add(Order);
                            }
                        }
                    }
                }
                resData.rData["rCode"] = 0;
                resData.rData["rMessage"] = "All cars found in Order successfully";
                resData.rData["Orders"] = OrdersList;
            }
            catch (Exception ex)
            {
                resData.rStatus = 402;
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = $"Exception occurred: {ex.Message}";
            }
            return resData;
        }
    }
}