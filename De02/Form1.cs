using De02.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace De02
{
    public partial class Form1 : Form
    {
        Model1 context = new Model1();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadData();
        }
        private void LoadData()
        {
            try
            {
                List<LoaiSP> listLoaisp = context.LoaiSPs.ToList();
                List<Sanpham> listSP = context.Sanphams.ToList();
                FillFaclutyCB(listLoaisp);
                BindGrid(listSP);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void FillFaclutyCB(List<LoaiSP> listLoaisp)
        {
            this.cmbLoaiSP.DataSource = listLoaisp;
            this.cmbLoaiSP.DisplayMember = "TenLoai";
            this.cmbLoaiSP.ValueMember = "MaLoai";
        }
        private void BindGrid(List<Sanpham> listSP)
        {
            var loaiSPs = context.LoaiSPs.ToList();
            dataGridView1.Rows.Clear();
            foreach (var item in listSP)
            {
                int index = dataGridView1.Rows.Add();
                dataGridView1.Rows[index].Cells[0].Value = item.MaSP;
                dataGridView1.Rows[index].Cells[1].Value = item.TenSP;
                dataGridView1.Rows[index].Cells[2].Value = item.NgayNhap;
                dataGridView1.Rows[index].Cells[3].Value = item.LoaiSP.TenLoai;
            }

        }
        private void ResetForm()
        {
            txtMaSP.Clear();
            txtTen.Clear();
            cmbLoaiSP.SelectedIndex = -1;
            dtNgayNhap.Value = DateTime.Now;
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            // Kiểm tra thông tin bắt buộc
            if (string.IsNullOrWhiteSpace(txtMaSP.Text) || string.IsNullOrWhiteSpace(txtTen.Text) || cmbLoaiSP.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return;
            }
            // Kiểm tra mã sp
            if (txtMaSP.Text.Length != 6)
            {
                MessageBox.Show("Mã sp phải có 6 kí tự!");
                return;
            }
            // Kiểm tra và tạo đối tượng sp
            Sanpham newSanPham = new Sanpham()
            {
                MaSP = txtMaSP.Text,
                TenSP = txtTen.Text,
                NgayNhap = dtNgayNhap.Value,
                MaLoai = cmbLoaiSP.SelectedValue.ToString().Substring(0, 2)
            };

            try
            {
                using (Model1 context = new Model1())
                {
                    // Thêm sinh viên vào CSDL
                    context.Sanphams.Add(newSanPham);
                    context.SaveChanges(); 
                }

                LoadData();
                ResetForm(); 
                MessageBox.Show("Thêm mới dữ liệu thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm dữ liệu: " + ex.Message);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            // Kiểm tra thông tin đầu vào
            if (string.IsNullOrWhiteSpace(txtMaSP.Text) || string.IsNullOrWhiteSpace(txtTen.Text) || string.IsNullOrWhiteSpace(dtNgayNhap.Text) || cmbLoaiSP.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin sản phẩm!");
                return;
            }
            // Kiểm tra ngày nhập hợp lệ
            if (!DateTime.TryParse(dtNgayNhap.Text, out DateTime ngayNhap))
            {
                MessageBox.Show("Ngày nhập không hợp lệ!");
                return;
            }

            // Tìm sản phẩm cần sửa
            var existingProduct = context.Sanphams.FirstOrDefault(sp => sp.MaSP == txtMaSP.Text);
            if (existingProduct == null)
            {
                MessageBox.Show("Mã sản phẩm không tồn tại!");
                return;
            }

            // Cập nhật thông tin sản phẩm
            existingProduct.TenSP = txtTen.Text;
            existingProduct.NgayNhap = ngayNhap;
            existingProduct.MaLoai = cmbLoaiSP.SelectedValue.ToString(); 


            context.SaveChanges();
            try
            {
               
                LoadData();
                ResetForm();
                MessageBox.Show("Cập nhật dữ liệu thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật dữ liệu: " + ex.Message);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaSP.Text))
            {
                MessageBox.Show("Vui lòng nhập mã sản phẩm cần xóa!");
                return;
            }

            using (var context = new Model1())
            {
                // Tìm sản phẩm cần xóa dựa trên mã sản phẩm
                var productToDelete = context.Sanphams.FirstOrDefault(sp => sp.MaSP == txtMaSP.Text);
                if (productToDelete == null)
                {
                    MessageBox.Show("Không tìm thấy sản phẩm cần xóa!");
                    return;
                }

                var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn xóa sản phẩm này không?",
                                                    "Xác nhận xóa!",
                                                    MessageBoxButtons.YesNo,
                                                    MessageBoxIcon.Warning);

                if (confirmResult == DialogResult.Yes)
                {
                    try
                    {
                        // Xóa sản phẩm
                        context.Sanphams.Remove(productToDelete);
                        context.SaveChanges(); 

                        LoadData();
                        ResetForm(); 

                        MessageBox.Show("Xóa sản phẩm thành công!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi xóa sản phẩm: " + ex.Message);
                    }
                }
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn thoát?", "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmResult == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            string searchTerm = txtTim.Text.Trim();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                MessageBox.Show("Vui lòng nhập tên sản phẩm để tìm kiếm!");
                return;
            }

            using (var context = new Model1())
            {
                // Lấy danh sách sản phẩm theo tên
                var filteredProducts = context.Sanphams
                    .Where(sp => sp.TenSP.Contains(searchTerm))
                    .ToList();

                // Bind dữ liệu vào DataGridView
                BindGrid(filteredProducts);

                if (filteredProducts.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy sản phẩm nào!");
                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Lấy dòng được chọn
                var selectedRow = dataGridView1.Rows[e.RowIndex];

                // Hiển thị dữ liệu lên các control tương ứng
                txtMaSP.Text = selectedRow.Cells[0].Value.ToString(); 
                txtTen.Text = selectedRow.Cells[1].Value.ToString();
                dtNgayNhap.Value = Convert.ToDateTime(selectedRow.Cells[2].Value); 
                cmbLoaiSP.Text = selectedRow.Cells[3].Value.ToString(); 
            }
        }
    }
}
