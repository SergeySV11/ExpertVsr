using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using OfficeOpenXml;
using Microsoft.Win32;

namespace Expert_VSR
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ScriptGenerator ScriptGeneratorExec { get; set; }
        public ScriptGenerator2 ScriptGeneratorExec2 { get; set; }
        private List<DataSumm> DataSumm = new List<DataSumm>();
        private string TypeRst;

        public MainWindow()
        {
            InitializeComponent();
            ScriptGeneratorExec = new ScriptGenerator("Persist Security Info=False;User ID=sa;Password=sa;Initial Catalog=ExpertXml;Server=depo");
            ScriptGeneratorExec2 = new ScriptGenerator2("Persist Security Info=False;User ID=sa;Password=sa;Initial Catalog=ExpertXml;Server=depo");
            Choice_RstType.Items.Add("Первичный");
            Choice_RstType.Items.Add("Повторный");
            Choice_RstType.Items.Add("Максимальный");
            Choice_RstType.SelectedIndex = 0;
        }

        private void Choice_RstType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TypeRst = "0";
            switch (Choice_RstType.SelectedIndex)
            {
                case 0:
                    TypeRst = "1";
                    break;
                case 1:
                    TypeRst = "2";
                    break;
                case 2:
                    TypeRst = "max";
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            if (!Ot_Per.Text.ToString().Contains("_"))
            {
                string sql_Summ = @"EXEC vsr_summa " + Ot_Per.Text.ToString() + ',' + TypeRst.ToString();
                ScriptGeneratorExec.ExecSelect(sql_Summ, out DataSumm);
                Data_Sum.ItemsSource = DataSumm;
            }
            else
            {
                MessageBox.Show("Вы не заполнили Ot_Per", "Сообщение");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (!Ot_Per.Text.ToString().Contains("_"))
            {
                string sql_Othet = @"EXEC vsr_Otchet " + Ot_Per.Text.ToString() + ',' + TypeRst.ToString();
                List<object> data = new List<object>();
                ScriptGeneratorExec2.ExecSelect(sql_Othet, out data);

                string epath = Environment.CurrentDirectory + @"\Template\Otchet.xlsx";
                FileInfo einfo = new FileInfo(epath);
                ExcelPackage pck = new ExcelPackage(einfo);
                ExpToExcel exp = new ExpToExcel(data, pck);

                SaveFileDialog dlg = new SaveFileDialog
                {
                    Title = "Выбор куда сохранить",
                    Filter = "Excel files (*.xlsx)|*.xlsx",
                    AddExtension = false
                };
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    string SaveFilePath = dlg.FileName;
                    if (File.Exists(SaveFilePath))
                        File.Delete(SaveFilePath);
                    pck.SaveAs(new FileInfo(SaveFilePath));
                    MessageBox.Show("Копирование выполнено", "Результат команды");
                }
                else MessageBox.Show("Ошибка копирования", "Результат команды");
            }
            else
            {
                MessageBox.Show("Вы не заполнили Ot_Per", "Сообщение");
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Prikriplenie Pr = new Prikriplenie();
            Pr.Show();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Spr spr = new Spr();
            spr.Show();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            Pisma Ps = new Pisma();
            Ps.Show();
        }
    }
}
