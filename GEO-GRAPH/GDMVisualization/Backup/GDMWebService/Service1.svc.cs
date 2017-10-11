using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Data.SqlClient;
using NLog;
using System.Web;
using System.Configuration;

namespace GDMWebService
{
    #region class Service1
    
    public class Service1 : IService1
    {
        #region Getter Functions

        /// <summary>
        /// Get the XML Graph of the experiment marked as "Current"
        /// </summary>
        /// <returns>Current Experiment Graph</returns>
        public string GetExperimentGraph()
        {
            return AccessLayer.GetExperimentGraph();
        }

        
        public string GetSessionVariable()
        {
            return (string)System.Web.HttpContext.Current.Session["userId"];
        }

        /// <summary>
        /// Get Solution Graph of the experiment marked as "Current"
        /// </summary>
        /// <returns>Current Solution Graph</returns>
        public byte[] GetSolutionGraph()
        {
            return AccessLayer.GetSolutionGraph();
        }

        /// <summary>
        /// Checks if this experiment is defined to show a solution window for the experiment marked as "Current"
        /// </summary>
        /// <returns>Time before show solution</returns>
        public string GetUseSolutionGraph()
        {
            return AccessLayer.GetUseSolutionGraph();
        }

        /// <summary>
        /// Get the Id of the experiment marked as "Current"
        /// </summary>
        /// <returns>Experiment ID</returns>
        public int GetCurrentExperimentID()
        {
            return AccessLayer.GetCurrentExperimentID();
        }

        #endregion


        #region Setter Functions

        /// <summary>       
        /// Insert the actions the Participant performed to the DB
        /// </summary>  
        public bool InsertIntoExperimentActions(int ExperimentID, int PersonID, string VertexID, string Color, DateTime EventTime)
        {
            return AccessLayer.InsertIntoExperimentActions(ExperimentID, PersonID, VertexID, Color, EventTime);
        }

        public object setSessionVariable(string key, string value)
        {
            return System.Web.HttpContext.Current.Session[key] = value;
        }
        /// <summary>
        /// Add a new particepant to the DB
        /// </summary>
        /// <returns>ID assigned to participant</returns>
        public int AddParticipant(Object turkID)
        {
            string id = turkID as string;
            return AccessLayer.AddParticipant(id);
        }

        /// <summary>
        /// Insert new Experiment graph to DB
        /// </summary>
        /// <returns>ID assigned to graph</returns>
        public int InsertExperimentGraph(string graph_xml)
        {
            return AccessLayer.InsertExperimentGraph(graph_xml);
        }

        /// <summary>
        /// Update Experiment Results for the experiment marked as "Current" with the participant's response 
        /// </summary>  
        public bool SolvedProblemGraph(int ParticipantID, bool Solved, DateTime SolvedTime)
        {
            return AccessLayer.SolvedProblemGraph(ParticipantID, Solved, SolvedTime);
        }

        #endregion


        #region Query/boolean Functions

        /// <summary>
        /// Inserts partial Experiment Results
        /// </summary>  
        public bool SolutionAcceptancePartial(int ParticipantID, int ExperimentID, DateTime Date, bool AcceptedSolution)
        {
            return AccessLayer.SolutionAcceptancePartial(ParticipantID, ExperimentID, Date, AcceptedSolution);
        }

        public bool PerformBulkCopy()
        {
            return AccessLayer.PerformBulkCopy();
        }

        #endregion                                          
    }

    #endregion

    /// <summary>
    /// Data Access Layer Class
    /// </summary> 
    public static class AccessLayer
    {
        static string connGDM = ConfigurationManager.ConnectionStrings["ConnectionGroupDM"].ConnectionString;
        static SqlConnection SqlCon = new SqlConnection(connGDM); 
            //new SqlConnection("Data Source=132.72.64.53;Initial Catalog=GroupDM;User Id=gdm;Password=a1234;");
        //static SqlConnection SqlCon = new SqlConnection("Data Source=URI-LAPTOP\\sqlexpress;Initial Catalog=GradProject;Integrated Security=SSPI;User Id=uri;Password=1nsoy7y;");
        static Logger logger = LogManager.GetLogger("VC");

        #region Getter Functions

        /// <summary>
        /// Get the Id of the experiment marked as "Current"
        /// </summary>
        /// <returns>Current Experiment ID </returns>
        public static int GetCurrentExperimentID()
        {
            int id = 1;

            string query = "SELECT ExperimentID FROM Experiment WHERE [Current] = 'true'";
            SqlCommand cmd = new SqlCommand(query, SqlCon);
                      
            try
            {
                SqlCon.Open();
                id = (int)cmd.ExecuteScalar();
            }
            catch { 
                id = 12; //TODO: hard-coded for curr exp
                logger.Error("GetCurrentExperimentID: sqlConnectionError");
            }
            finally { SqlCon.Close(); }

            
            return id;
        }

        /// <summary>
        /// Get the XML Graph of the experiment marked as "Current"
        /// </summary>
        /// <returns>Current Experiment Graph</returns>
        public static string GetExperimentGraph()
        {
            int ExperimentID = GetCurrentExperimentID();

            string graph = "";
            // Map SQL param names to C# param names
            SqlParameter sql_ExperimentID = new SqlParameter("ExperimentID", ExperimentID);

            string query = "SELECT InputGraph FROM Experiment WHERE ExperimentID = @ExperimentID";
            SqlCommand cmd = new SqlCommand(query, SqlCon);
            
            // Add Params to the query string
            cmd.Parameters.Add(sql_ExperimentID);

            try
            {
                SqlCon.Open();
                graph = (string)cmd.ExecuteScalar();
            }
            catch { graph = "error loading graph";
            logger.Error("GetExperimentGraph: sqlConnectionError");
            }
            finally { SqlCon.Close(); }

            return graph;
        }

        /// <summary>
        /// Get Solution Graph of the experiment marked as "Current"
        /// </summary>
        /// <returns>Current Solution Graph</returns>
        public static byte[] GetSolutionGraph()
        {
            int ExperimentID = GetCurrentExperimentID();

            byte[] graph = {};
            // Map SQL param names to C# param names
            SqlParameter sql_ExperimentID = new SqlParameter("ExperimentID", ExperimentID);
  
            string query = "SELECT SolutionGraph FROM Experiment WHERE ExperimentID = @ExperimentID";
            SqlCommand cmd = new SqlCommand(query, SqlCon);
            
            // Add Params to the query string
            cmd.Parameters.Add(sql_ExperimentID);

            try
            {
                SqlCon.Open();
                graph = (byte[])cmd.ExecuteScalar();
            }
            catch { logger.Error("GetSolutionGraph: sqlConnectionError"); }
            finally { SqlCon.Close(); }

            return graph;
        }
                
        /// <summary>
        /// Checks if this experiment is defined to show a solution window for the experiment marked as "Current"
        /// </summary>
        /// <returns>Time before show solution</returns>
        public static string GetUseSolutionGraph()
        {
            int ExperimentID = GetCurrentExperimentID();

            string ans = "0";
            // Map SQL param names to C# param names
            SqlParameter sql_ExperimentID = new SqlParameter("ExperimentID", ExperimentID);

            string query = "SELECT TimeTillSolution FROM Experiment WHERE ExperimentID = @ExperimentID";
            SqlCommand cmd = new SqlCommand(query, SqlCon);

            // Add Params to the query string
            cmd.Parameters.Add(sql_ExperimentID);

            try
            {
                SqlCon.Open();
                ans = (string)cmd.ExecuteScalar();
            }
            catch { logger.Error("GetUseSolutionGraph: sqlConnectionError"); }
            finally { SqlCon.Close(); }

            return ans;
        }

        #endregion


        #region Setter Functions

        /// <summary>       
        /// Insert the actions the Participant performed to the DB
        /// </summary>       
        public static bool InsertIntoExperimentActions(int ExperimentID, int PersonID, string VertexID, string Color, DateTime EventTime)
        {
            bool ans = true;
            // Map SQL param names to C# param names
            SqlParameter sql_ExperimentID = new SqlParameter("ExperimentID", ExperimentID);
            SqlParameter sql_PersonID = new SqlParameter("PersonID", PersonID);
            SqlParameter sql_VertexID = new SqlParameter("VertexID", VertexID);
            SqlParameter sql_Color = new SqlParameter("Color", Color);
            SqlParameter sql_EventTime = new SqlParameter("EventTime", DateTime.Now);

            string query = "INSERT INTO ExperimentActions VALUES (@ExperimentID, @PersonID, @VertexID, @Color, @EventTime)";
            SqlCommand cmd = new SqlCommand(query, SqlCon);

            // Add Params to the query string
            cmd.Parameters.Add(sql_ExperimentID);
            cmd.Parameters.Add(sql_PersonID);
            cmd.Parameters.Add(sql_VertexID);
            cmd.Parameters.Add(sql_Color);
            cmd.Parameters.Add(sql_EventTime);
            try
            {
                SqlCon.Open();
                cmd.ExecuteNonQuery();
            }
            catch { ans = false;
            logger.Error("ExperimentActions: sqlConnectionError");
            }
            finally { SqlCon.Close(); }
            //logger.Info("Action: user={0},experiment={1},vertexId={2},color={3},time={4}", PersonID,ExperimentID,VertexID,Color,EventTime);
            logger.Info("Action,{0},{1},{2},{3},{4}", PersonID, ExperimentID, VertexID, Color, EventTime);
            return ans;
        }

        /// <summary>
        /// Add a new particepant to the DB
        /// </summary>
        /// <returns>ID assigned to participant</returns>
        public static int AddParticipant(string TurkID)
        {
            
            SqlParameter sql_TurkID = new SqlParameter("TurkID", TurkID);
            string query = "INSERT INTO Participant (TurkID) VALUES ('"+TurkID+"')";
            SqlCommand cmd = new SqlCommand(query, SqlCon);
            try
            {
                SqlCon.Open();
                cmd.ExecuteNonQuery();
            }
            catch { logger.Error("Add: sqlConnectionError"); }
            finally { SqlCon.Close(); }
            Random r = new Random();
            int id = r.Next(1000,2000) * 10;
            query = "SELECT MAX(ParticipantID) FROM Participant";
            SqlCommand cmd2 = new SqlCommand(query, SqlCon);
            try
            {
                SqlCon.Open();
                id = (int)cmd2.ExecuteScalar();
            }
            catch { logger.Error("Add: sqlConnectionError"); }
            finally { SqlCon.Close(); }

            //logger.Info("Added: id={0}, code={1}", id,TurkID);
            logger.Info("Add,{0},{1}", id, TurkID);
            
            return id;
        }

        /// <summary>
        /// Insert new Experiment graph to DB
        /// </summary>
        /// <returns>ID assigned to graph</returns>
        public static int InsertExperimentGraph(string graph_xml)
        {
            // Map SQL param names to C# param names
            SqlParameter sql_InputGraph = new SqlParameter("InputGraph", graph_xml);

            string query = "Insert INTO InputGraph (InputGraph) VALUES (@InputGraph)";
            SqlCommand cmd = new SqlCommand(query, SqlCon);

            // Add Params to the query string
            cmd.Parameters.Add(sql_InputGraph);

            int id = 0;

            try
            {
                SqlCon.Open();
                cmd.ExecuteNonQuery();
            }
            catch { logger.Error("InsertExperimentGraph: sqlConnectionError"); }
            finally
            {
                SqlCon.Close();

                // Get id of just created graph

                query = "SELECT MAX(GraphID) FROM InputGraph";
                SqlCommand cmd2 = new SqlCommand(query, SqlCon);
                try
                {
                    SqlCon.Open();
                    id = (int)cmd2.ExecuteScalar();
                }
                catch { logger.Error("InsertExperimentGraph: sqlConnectionError"); }
                finally { SqlCon.Close(); }
            }
            return id;
        }

        /// <summary>
        /// Update Experiment Results for the experiment marked as "Current" with the participant's solution 
        /// </summary>  
        public static bool SolvedProblemGraph(int ParticipantID, bool Solved, DateTime SolvedTime)
        {
            int ExperimentID = GetCurrentExperimentID();

            // Check if there is an entery for this experiment and participant
            bool entery = CheckExperimentResultsEntery(ParticipantID);

            string query;
            bool ans = true;
            // Map SQL param names to C# param names
            SqlParameter sql_ParticipantID = new SqlParameter("ParticipantID", ParticipantID);
            SqlParameter sql_ExperimentID = new SqlParameter("ExperimentID", ExperimentID);
            SqlParameter sql_Solved = new SqlParameter("Solved", Solved);
            SqlParameter sql_SolvedTime = new SqlParameter("SolvedTime", DateTime.Now);

            if (entery)
            {
                // If entery allready exists - update this entery
                query = "UPDATE ExperimentResult SET Solved = @Solved, SolvedTime = @SolvedTime " +
                "WHERE ParticipantID = @ParticipantID AND ExperimentID = @ExperimentID";
            }
            else
            {
                // If entery doesn't exist - create a new entery            
                query = "INSERT INTO ExperimentResult (ParticipantID, ExperimentID, Solved, SolvedTime) " +
                    "VALUES (@ParticipantID, @ExperimentID, @Solved, @SolvedTime)";
            }
            SqlCommand cmd = new SqlCommand(query, SqlCon);

            // Add Params to the query string
            cmd.Parameters.Add(sql_ParticipantID);
            cmd.Parameters.Add(sql_ExperimentID);
            cmd.Parameters.Add(sql_Solved);
            cmd.Parameters.Add(sql_SolvedTime);
            try
            {
                SqlCon.Open();
                cmd.ExecuteNonQuery();
            }
            catch { ans = false;
            logger.Error("Results: sqlConnectionError");
            }
            finally { SqlCon.Close(); }
            //logger.Info("Results: user={0},experiment={1},solved={2},time={3}", ParticipantID, ExperimentID, Solved, SolvedTime);
            logger.Info("Results,{0},{1},{2},{3}", ParticipantID, ExperimentID, Solved, SolvedTime);
            return ans;
        }

        /// <summary>
        /// Inserts partial Experiment Results
        /// </summary>  
        public static bool SolutionAcceptancePartial(int ParticipantID, int ExperimentID, DateTime Date, bool AcceptedSolution)
        {
            // Check if there is an entery for this experiment and participant
            bool entery = CheckExperimentResultsEntery(ParticipantID);

            string query;

            bool ans = true;
            // Map SQL param names to C# param names
            SqlParameter sql_ParticipantID = new SqlParameter("ParticipantID", ParticipantID);
            SqlParameter sql_ExperimentID = new SqlParameter("ExperimentID", ExperimentID);
            SqlParameter sql_Date = new SqlParameter("Date", Date);
            SqlParameter sql_AcceptedSolution = new SqlParameter("AcceptedSolution", AcceptedSolution);

            if (entery)
            {
                // If yes - query to update this entery
                query = "UPDATE ExperimentResult SET Date = @Date, AcceptedSolution = @AcceptedSolution " +
                "WHERE ParticipantID = @ParticipantID AND ExperimentID = @ExperimentID";
            }
            else
            {
                // If not - query to create a new one            
                query = "INSERT INTO ExperimentResult (ParticipantID, ExperimentID, Date, AcceptedSolution) " +
               "VALUES (@ParticipantID, @ExperimentID, @Date, @AcceptedSolution)";
            }

            SqlCommand cmd = new SqlCommand(query, SqlCon);

            // Add Params to the query string
            cmd.Parameters.Add(sql_ParticipantID);
            cmd.Parameters.Add(sql_ExperimentID);
            cmd.Parameters.Add(sql_Date);
            cmd.Parameters.Add(sql_AcceptedSolution);
            try
            {
                SqlCon.Open();
                cmd.ExecuteNonQuery();
            }
            catch { ans = false;
            logger.Error("SolutionAcceptancePartial: sqlConnectionError");
            }
            finally { SqlCon.Close(); }
            return ans;
        }

        #endregion


        #region Boolean Functions
       
        /// <summary>
        /// Check if there allready exists an entery for this participant and for the experiment marked as "Current"        
        /// </summary>
        /// <param name="ParticipantID"></param>
        /// <returns></returns>
        public static bool CheckExperimentResultsEntery(int ParticipantID)
        {
            int ExperimentID = GetCurrentExperimentID();

            bool ans = true;
            // Map SQL param names to C# param names
            SqlParameter sql_ParticipantID = new SqlParameter("ParticipantID", ParticipantID);
            SqlParameter sql_ExperimentID = new SqlParameter("ExperimentID", ExperimentID);
            
            string query = "SELECT ParticipantID FROM ExperimentResult WHERE ParticipantID = @ParticipantID AND ExperimentID = @ExperimentID";
            SqlCommand cmd = new SqlCommand(query, SqlCon);

            // Add Params to the query string
            cmd.Parameters.Add(sql_ParticipantID);
            cmd.Parameters.Add(sql_ExperimentID);

            try
            {
                SqlCon.Open();
                if (cmd.ExecuteScalar() != null)
                {
                    //int id = (int)cmd.ExecuteScalar();
                    ans = true;
                }
                else
                    ans = false;
            }
            catch { }
            finally { SqlCon.Close(); }

            return ans;
        }       

        public static bool PerformBulkCopy()
        {            
            bool result = true;

            try
            {
                // get the source data
                string connGP = ConfigurationManager.ConnectionStrings["ConnectionGarndP"].ConnectionString;
                using (SqlConnection sourceConnection = new SqlConnection(connGP))
                    
                   // new SqlConnection("Data Source=URI-LAPTOP\\sqlexpress;Initial Catalog=GradProject;Integrated Security=SSPI;User Id=uri;Password=1nsoy7y;"))
                {
                    SqlCommand myCommand = new SqlCommand("SELECT * FROM dbo.SolutionGraph", sourceConnection);
                    sourceConnection.Open();
                    SqlDataReader reader = myCommand.ExecuteReader();

                    // open the destination data
                     string connGDM = ConfigurationManager.ConnectionStrings["ConnectionGroupDM"].ConnectionString;
                    using (SqlConnection destinationConnection =
                              new SqlConnection (connGDM))
                        //new SqlConnection("Data Source=132.72.64.53;Initial Catalog=GroupDM;User Id=gdm;Password=a1234;"))
                    {
                        // open the connection
                        destinationConnection.Open();


                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(destinationConnection))
                        {
                            bulkCopy.BatchSize = 500;
                            bulkCopy.NotifyAfter = 1000;
                            //bulkCopy.SqlRowsCopied += new SqlRowsCopiedEventHandler(bulkCopy_SqlRowsCopied);
                            bulkCopy.DestinationTableName = "dbo.SolutionGraph";
                            bulkCopy.WriteToServer(reader);
                        }
                    }
                    reader.Close();
                }               
            }
            catch { result = false; }

            return result;
        }

        #endregion

    }
}
