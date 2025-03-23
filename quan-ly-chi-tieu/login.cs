using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

// password suggestion messagebox, password suggestion + copy to clipboard, change password


namespace quan_ly_chi_tieu
{
    public partial class login : Form // vu ha vy
    {
        public readonly string connectionString;
        public int userID;
        public login()
        {
            InitializeComponent();
            this.connectionString = "Data Source=KlausJackson\\SQLEXPRESS;Initial Catalog=qlct;Integrated Security=True;";
        }

        private int authentication()
        {
            if (!checkTb()) return -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT userID FROM Users WHERE username = @username AND passwd = @passwd";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", txUsername.Text);
                    command.Parameters.AddWithValue("@passwd", hashPassword(txPasswd.Text));
                    if (command.ExecuteScalar() == null)
                    {
                        MessageBox.Show("Tên người dùng hoặc mật khẩu không đúng", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return -1;
                    }
                    return (int)command.ExecuteScalar();
                }
            }
        }

        private void DeleteRelatedRecords(string tableName, int userid)
        {
            string query = $"DELETE FROM {tableName} WHERE userID = @userid";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userid", userid);
                    command.ExecuteNonQuery();
                }
            }

        }
        private bool checkTb()
        {
            if (string.IsNullOrWhiteSpace(txUsername.Text) || string.IsNullOrWhiteSpace(txPasswd.Text))
            {
                MessageBox.Show("Vui lòng nhập tên người dùng và mật khẩu", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void delUser_Click(object sender, EventArgs e)
        {
            int userID = authentication();
            if (userID == -1) return;

            foreach (Form form in Application.OpenForms)
            {
                if (form is main)
                {
                    int main_userID = (form as main).userID;
                    if (main_userID == userID)
                    {
                        MessageBox.Show("Không thể xóa người dùng đang đăng nhập", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            }

            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa người dùng này? Mọi dữ liệu của người dùng này sẽ bị xóa!", "Cảnh báo", 
            MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.No) return;
            try
            {
                DeleteRelatedRecords("expenses", userID);
                DeleteRelatedRecords("income", userID);                
                DeleteRelatedRecords("categories", userID);
                DeleteRelatedRecords("users", userID);
                MessageBox.Show("Người dùng đã được xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa người dùng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void signUp_Click(object sender, EventArgs e)
        {
            if (!checkTb()) return;

            string username = txUsername.Text;
            string password = txPasswd.Text;
            if (username.Length > 30 || password.Length > 100)
            {
                MessageBox.Show("Tên người dùng không được vượt quá 30 ký tự.\nMật khẩu không được vượt quá 100 ký tự.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!Regex.IsMatch(username, @"^[a-zA-Z0-9_ ]+$"))
            {
                MessageBox.Show("Tên người dùng chỉ được chứa chữ cái, số, dấu gạch dưới và khoảng trắng", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "select 1 FROM Users WHERE username = @username";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        if (command.ExecuteScalar() != null)
                        {
                            MessageBox.Show("Tên người dùng đã tồn tại", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        else
                        {
                            query = "INSERT INTO Users (username, passwd) VALUES (@username, @passwd)";
                            command.CommandText = query;
                            command.Parameters.AddWithValue("@passwd", hashPassword(password));
                            if (command.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Đăng ký thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private string hashPassword(string password)
        {
            SHA256 sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++) builder.Append(bytes[i].ToString("x2"));
            return builder.ToString();
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            if (!checkTb()) return;
            try
            {
                int userID = authentication();
                if (userID == -1) return;
                this.userID = userID;
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void login_Load(object sender, EventArgs e)
        {

        }
    }
}
