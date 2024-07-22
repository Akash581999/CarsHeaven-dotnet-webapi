using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace MyCommonStructure.Services
{
    public class wishlists
    {
        dbServices ds = new dbServices();
        public async Task<responseData> AddToWishList(requestData rData)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                MySqlParameter[] checkParams = new MySqlParameter[]
                {
                    new MySqlParameter("@UserId", rData.addInfo["UserId"]),
                    new MySqlParameter("@DroneId", rData.addInfo["DroneId"])
                };

                var checkQuery = @"SELECT * FROM pc_student.TEDrones_Wishlists WHERE UserId=@UserId AND DroneId = @DroneId;";
                var dbCheckData = ds.ExecuteSQLName(checkQuery, checkParams);
                if (dbCheckData[0].Count() != 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Drone with this Id already exists in Wishlist!";
                }
                else
                {
                    MySqlParameter[] insertParams = new MySqlParameter[]
                    {
                        new MySqlParameter("@UserId", rData.addInfo["UserId"]),
                        new MySqlParameter("@DroneId", rData.addInfo["DroneId"]),
                        new MySqlParameter("@Quantity", rData.addInfo["Quantity"]),
                        new MySqlParameter("@Price", rData.addInfo["Price"]),
                        new MySqlParameter("@TotalPrice", rData.addInfo["TotalPrice"]),
                    };
                    var insertQuery = @"INSERT INTO pc_student.TEDrones_Wishlists (UserId, DroneId, Quantity, Price, TotalPrice) 
                                        VALUES (@UserId, @DroneId, @Quantity, @Price, @TotalPrice);";
                    int rowsAffected = ds.ExecuteInsertAndGetLastId(insertQuery, insertParams);

                    if (rowsAffected > 0)
                    {
                        resData.eventID = rData.eventID;
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "Drone added to Wishlist successfully.";
                    }
                    else
                    {
                        resData.rData["rCode"] = 3;
                        resData.rData["rMessage"] = "Failed to add drone to Wishlist!";
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

        public async Task<responseData> UpdateInWishList(requestData rData)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                MySqlParameter[] checkParams = new MySqlParameter[]
                {
                    new MySqlParameter("@WishlistId", rData.addInfo["WishlistId"]),
                    new MySqlParameter("@UserId", rData.addInfo["UserId"]),
                    new MySqlParameter("@DroneId", rData.addInfo["DroneId"]),
                };

                var query = @"SELECT * FROM pc_student.TEDrones_Wishlists WHERE WishlistId=@WishlistId AND UserId=@UserId AND DroneId=@DroneId";
                var dbData = ds.ExecuteSQLName(query, checkParams);
                if (dbData[0].Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Drone not found in Wishlist!";
                }
                else
                {
                    MySqlParameter[] updateParams = new MySqlParameter[]
                   {
                        new MySqlParameter("@WishlistId", rData.addInfo["WishlistId"]),
                        new MySqlParameter("@UserId", rData.addInfo["UserId"]),
                        new MySqlParameter("@DroneId", rData.addInfo["DroneId"]),
                        new MySqlParameter("@Quantity", rData.addInfo["Quantity"]),
                        new MySqlParameter("@Price", rData.addInfo["Price"]),
                        new MySqlParameter("@TotalPrice", rData.addInfo["TotalPrice"]),
                   };
                    var updatequery = @"UPDATE pc_student.TEDrones_Wishlists
                                        SET UserId = @UserId, DroneId = @DroneId, Quantity = @Quantity, Price = @Price, TotalPrice = @TotalPrice
                                        WHERE WishlistId = @WishlistId;";
                    var updatedata = ds.ExecuteInsertAndGetLastId(updatequery, updateParams);
                    if (updatedata != 0)
                    {
                        resData.rData["rCode"] = 3;
                        resData.rData["rMessage"] = "Some error occured, couldn't update Wishlist products!";
                    }
                    else
                    {
                        resData.eventID = rData.eventID;
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "Wishlist products updated successfully.";
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

        public async Task<responseData> RemoveFromWishList(requestData rData)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                // int WishlistId = Convert.ToInt32(rData.addInfo["WishlistId"]);
                // int droneId = Convert.ToInt32(rData.addInfo["DroneId"]);
                string WishlistId = rData.addInfo["WishlistId"].ToString();
                string UserId = rData.addInfo["UserId"].ToString();
                string DroneId = rData.addInfo["DroneId"].ToString();

                MySqlParameter[] para = new MySqlParameter[]
                {
                    new MySqlParameter("@WishlistId", WishlistId),
                    new MySqlParameter("@UserId", UserId),
                    new MySqlParameter("@DroneId", DroneId)
                };

                var query = @"SELECT * FROM pc_student.TEDrones_Wishlists WHERE WishlistId = @WishlistId AND UserId=@UserId AND DroneId = @DroneId;";
                var dbData = ds.ExecuteSQLName(query, para);
                if (dbData.Count == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Drone not found in the Wishlist!";
                }
                else
                {
                    para = new MySqlParameter[]
                    {
                        new MySqlParameter("@WishlistId", WishlistId),
                        new MySqlParameter("@UserId", UserId),
                        new MySqlParameter("@DroneId", DroneId)
                    };

                    var deleteSql = @"DELETE FROM pc_student.TEDrones_Wishlists WHERE WishlistId = @WishlistId AND UserId=@UserId AND DroneId = @DroneId;";
                    int rowsAffected = ds.ExecuteInsertAndGetLastId(deleteSql, para);
                    if (rowsAffected > 0)
                    {
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "Drone removed from Wishlist successfully.";
                    }
                    else
                    {
                        resData.rData["rCode"] = 2;
                        resData.rData["rMessage"] = "Failed to remove drone from Wishlist!";
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

        public async Task<responseData> GetAWishListCar(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            resData.eventID = req.eventID;
            resData.rData["rMessage"] = "Drone found successfully!";
            try
            {
                string WishlistId = req.addInfo["WishlistId"].ToString();
                string UserId = req.addInfo["UserId"].ToString();
                string DroneId = req.addInfo["DroneId"].ToString();

                MySqlParameter[] myParams = new MySqlParameter[]
                {
                    new MySqlParameter("@WishlistId", req.addInfo["WishlistId"]),
                    new MySqlParameter("@UserId", req.addInfo["UserId"]),
                    new MySqlParameter("@DroneId", req.addInfo["DroneId"])
                };

                string getsql = $"SELECT * FROM pc_student.TEDrones_Wishlists " +
                             "WHERE WishlistId = @WishlistId AND UserId = @UserId AND DroneId = @DroneId;";
                var Wishlistdata = ds.ExecuteSQLName(getsql, myParams);
                if (Wishlistdata == null || Wishlistdata.Count == 0 || Wishlistdata[0].Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Drone not found!";
                }
                else
                {
                    var WishlistData = Wishlistdata[0][0];
                    resData.rData["WishlistId"] = WishlistData["WishlistId"];
                    resData.rData["UserId"] = WishlistData["UserId"];
                    resData.rData["DroneId"] = WishlistData["DroneId"];
                    resData.rData["Quantity"] = WishlistData["Quantity"];
                    resData.rData["Price"] = WishlistData["Price"];
                    resData.rData["TotalPrice"] = WishlistData["TotalPrice"];
                }
            }
            catch (Exception ex)
            {
                resData.rStatus = 402;
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = ex + "Enter correct Wishlistid or DroneId or UserId!";
            }
            return resData;
        }

        public async Task<responseData> GetAllWishListCars(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            resData.eventID = req.eventID;
            try
            {
                var query = @"SELECT * FROM pc_student.TEDrones_Wishlists ORDER BY WishlistId ASC";
                var dbData = ds.executeSQL(query, null);
                if (dbData == null)
                {
                    resData.rData["rMessage"] = "Some error occurred, can't get all Wishlists!";
                    resData.rStatus = 1;
                    return resData;
                }

                List<object> WishlistsList = new List<object>();
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
                                var Wishlist = new
                                {
                                    WishlistId = rowData.ElementAtOrDefault(0),
                                    UserId = rowData.ElementAtOrDefault(1),
                                    DroneId = rowData.ElementAtOrDefault(2),
                                    Quantity = rowData.ElementAtOrDefault(3),
                                    Price = rowData.ElementAtOrDefault(4),
                                    TotalPrice = rowData.ElementAtOrDefault(5)
                                };
                                WishlistsList.Add(Wishlist);
                            }
                        }
                    }
                }
                resData.rData["rCode"] = 0;
                resData.rData["rMessage"] = "All Wishlists found successfully";
                resData.rData["Wishlists"] = WishlistsList;
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