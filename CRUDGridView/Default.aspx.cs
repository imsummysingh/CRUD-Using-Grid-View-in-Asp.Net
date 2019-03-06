using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

namespace CRUDGridView
{
    public partial class Default : System.Web.UI.Page
    {
        string ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=PhoneBookDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                populateGridView();
            }
        }

        void populateGridView()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                SqlDataAdapter sda = new SqlDataAdapter("SELECT * FROM PhoneBook",con);
                sda.Fill(dt);
            }
            if (dt.Rows.Count > 0)
            {
                gvPhoneBook.DataSource = dt;
                gvPhoneBook.DataBind();
            }
            else
            {
                dt.Rows.Add(dt.NewRow());
                gvPhoneBook.DataSource = dt;
                gvPhoneBook.DataBind();
                gvPhoneBook.Rows[0].Cells.Clear();
                gvPhoneBook.Rows[0].Cells.Add(new TableCell());
                gvPhoneBook.Rows[0].Cells[0].ColumnSpan = dt.Columns.Count;
                gvPhoneBook.Rows[0].Cells[0].Text = "No Data Found";
                gvPhoneBook.Rows[0].Cells[0].HorizontalAlign = HorizontalAlign.Center;
            }
        }

        protected void gvPhoneBook_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName.Equals("AddNew"))
                {
                    using (SqlConnection con = new SqlConnection(ConnectionString))
                    {
                        con.Open();
                        string query = "Insert into PhoneBook (FirstName,Lastname,Contact,Email) values (@FirstName,@LastName,@Contact,@Email)";
                        SqlCommand cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@FirstName", (gvPhoneBook.FooterRow.FindControl("txtFirstNameFooter") as TextBox).Text);
                        cmd.Parameters.AddWithValue("@LastName", (gvPhoneBook.FooterRow.FindControl("txtLastNameFooter") as TextBox).Text);
                        cmd.Parameters.AddWithValue("@Contact", (gvPhoneBook.FooterRow.FindControl("txtContactFooter") as TextBox).Text);
                        cmd.Parameters.AddWithValue("@Email", (gvPhoneBook.FooterRow.FindControl("txtEmailFooter") as TextBox).Text);
                        cmd.ExecuteNonQuery();
                        populateGridView();
                        lblSuccess.Text = "New Record Added";
                        lblError.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                lblSuccess.Text = "";
                lblError.Text = ex.Message;
            }
        }

        protected void gvPhoneBook_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvPhoneBook.EditIndex = e.NewEditIndex;
            populateGridView();
        }

        protected void gvPhoneBook_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvPhoneBook.EditIndex = -1;
            populateGridView();
        }

        protected void gvPhoneBook_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                    using (SqlConnection con = new SqlConnection(ConnectionString))
                    {
                        con.Open();
                        string query = "Update PhoneBook SET FirstName=@FirstName,Lastname=@LastName,Contact=@Contact,Email=@Email WHERE PhoneBookID=@id";
                        SqlCommand cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@FirstName", (gvPhoneBook.Rows[e.RowIndex].FindControl("txtFirstName") as TextBox).Text);
                        cmd.Parameters.AddWithValue("@LastName", (gvPhoneBook.Rows[e.RowIndex].FindControl("txtLastName") as TextBox).Text);
                        cmd.Parameters.AddWithValue("@Contact", (gvPhoneBook.Rows[e.RowIndex].FindControl("txtContact") as TextBox).Text);
                        cmd.Parameters.AddWithValue("@Email", (gvPhoneBook.Rows[e.RowIndex].FindControl("txtEmail") as TextBox).Text);
                        cmd.Parameters.AddWithValue("@id", Convert.ToInt32(gvPhoneBook.DataKeys[e.RowIndex].Value.ToString()));
                        cmd.ExecuteNonQuery();
                        gvPhoneBook.EditIndex = -1;
                        populateGridView();
                        lblSuccess.Text = "Selected Row Updated";
                        lblError.Text = "";
                    }
            }
            catch (Exception ex)
            {
                lblSuccess.Text = "";
                lblError.Text = ex.Message;
            }
        }

        protected void gvPhoneBook_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    string query = "Delete From PhoneBook WHERE PhoneBookID=@id";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@id", Convert.ToInt32(gvPhoneBook.DataKeys[e.RowIndex].Value.ToString()));
                    cmd.ExecuteNonQuery();
                    populateGridView();
                    lblSuccess.Text = "Selected Row Deleted";
                    lblError.Text = "";
                }
            }
            catch (Exception ex)
            {
                lblSuccess.Text = "";
                lblError.Text = ex.Message;
            }
        }
    }
}