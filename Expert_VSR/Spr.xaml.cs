using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using Microsoft.Win32;


namespace Expert_VSR
{
    /// <summary>
    /// Логика взаимодействия для Spr.xaml
    /// </summary>
    public partial class Spr : Window
    {
        public ScriptGenerator ScriptGeneratorExec { get; set; }
        public string ot_Per;
        public Spr()
        {
            InitializeComponent();
            ScriptGeneratorExec = new ScriptGenerator("Persist Security Info=False;User ID=sa;Password=sa;Initial Catalog=NSI;Server=depo");
            Combo_Sp.Items.Add("Услуги");
            Combo_Sp.Items.Add("ЛПУ плательщики");
            Combo_Sp.Items.Add("ЛПУ исполнители");
            Combo_Sp.Items.Add("ЛПУ-звено направления");
            Combo_Sp.Items.Add("Услуга-звено направления");
            Combo_Sp.Items.Add("Диагностические услуги");
            Combo_Sp.SelectedIndex = 0;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            #region Visual_SP
            if (Ot_Per.Text.ToString().Contains("_"))
            {
                MessageBox.Show("Вы не заполнили Ot_Per", "Сообщение");
                Console.Beep();
            }
            else
            {
                Grid.Visibility = Visibility;
                List<GetString> NameTb = new List<GetString>();
                List<Tbl> Tbl = new List<Tbl>();
                string sql_NameTb = @"select cast(column_name as Char) as SqlString From information_schema.columns 
                                Where table_name=(SELECT Top 1 'Vsr_'+Name_Sp FROM [NSI].[dbo].[Sp_NSI] WHERE Name_Sp2='" + Combo_Sp.SelectedValue.ToString() + @"')";
                ScriptGeneratorExec.ExecSelect(sql_NameTb, out NameTb);

                string sql_Table = @"DECLARE @tt Char(1000) ,@Id_Nsi Char(10)
                               (SELECT @tt=Sql_vyvod ,@Id_Nsi=Id_Nsi FROM [NSI].[dbo].[Sp_NSI] WHERE Name_Sp2='" + Combo_Sp.SelectedValue.ToString() + @"' AND '" + Ot_Per.Text.ToString() + @"' BETWEEN Ot_Per1 AND Ot_Per2)
                               If @tt<>''
                               Begin
                               EXEC(@tt+'Where Id_Nsi='+ @Id_Nsi)
                               end";
                ScriptGeneratorExec.ExecSelect(sql_Table, out Tbl);
                Grid.ItemsSource = Tbl;

                for (int n = 0; n < 7; n++)
                {
                    if (n < NameTb.Count)
                    {
                        Grid.Columns[n].Header = NameTb[n].SqlString.ToString();
                    }
                    else
                    {
                        Grid.Columns[n].Header = "";
                    }
                }
            }
            #endregion 
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (Ot_Per.Text.ToString().Contains("_"))
            {
                MessageBox.Show("Вы не заполнили Ot_Per", "Сообщение");
            }
            else
            {   
                //выбор файла Excel
                OpenFileDialog dlg = new OpenFileDialog
                {
                    Title = "Выбор файла",
                    Filter = "Excel files (*.xlsx)|*.xlsx",
                    AddExtension = true
                };
                Nullable<bool> result = dlg.ShowDialog();
                List<GetString> Id = new List<GetString>();
                if (result == true)
                {
                    string patch = dlg.FileName;
                    string Combo = Combo_Sp.SelectedValue.ToString();
                    string ot_Per = Ot_Per.Text.ToString();
                    
                    //добавить запись в справочник справочников
                    string sql2 = @"insert into [NSI].[dbo].[Sp_NSI] 
                                  Select '' As Name_Sp ,"+ "'" + Combo_Sp.SelectedValue.ToString() + "'"+ @" AS Name_Sp2 ,'1.0'[version] ,cast (GetDate() As Date) As [Date] ,cast (GetDate() As Date) As Date_Obnov ,"+ "'" + Ot_Per.Text.ToString() + "'" + @" as Ot_Per1 ,'' As Ot_Per2 ,'' As Sql_vyvod
                                  Select Top 1 cast(Id_NSI as Char) SqlString From [NSI].[dbo].[Sp_NSI] Where Name_Sp2=" + "'" + Combo_Sp.SelectedValue.ToString()+ "'" + @" and Name_Sp='' 
                                  Order by [Date] Desc";
                    ScriptGeneratorExec.ExecSelect(sql2 ,out Id);
                    string Id_Nsi = Id[0].SqlString.ToString();

                    ImpToExcel imp = new ImpToExcel(patch);             //запускаем парсер указав путь к файлу  


                    foreach (DataRow row in imp.Imp_Table.Rows)
                    {
                        // получаем все ячейки строки
                        string Values = string.Empty;
                        var cells = row.ItemArray;
                        foreach (var cel in cells)
                        {
                            Values += "'" + cel + "',";
                        }
                        try
                        {
                            if (Values != "")
                            {
                                string sql = @"EXECUTE [ExpertXml].[dbo].[Vsr_Import] '"+ Id_Nsi+ "'," + Values + "'" + Combo + "','" + ot_Per + "'";    //добавляем записи в справочник
                                ScriptGeneratorExec.ExecScript(sql);
                            }
                        }

                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            //удаляем в случае ошибки
                            string sql4 = @"Delete [NSI].[dbo].[Sp_NSI] Where Id_Nsi='" + Id_Nsi + @"'
                                                Declare @sql varchar(500)
			                                          ,@tab varchar(25)= (SELECT Top 1 'Vsr_'+Name_Sp FROM [NSI].[dbo].[Sp_NSI] WHERE Name_Sp2='" + Combo + @"')	
		                                        set @sql = 'Delete '+Trim(@tab)+' Where Id_Nsi=" + Id_Nsi + @"'
                                                EXEC (@sql)";
                            ScriptGeneratorExec.ExecScript(sql4);
                            break;
                        }
                    }

                    string sql3 = @"Update[NSI].[dbo].[Sp_NSI]
                                    Set Ot_Per2 = '9999', Name_Sp = (Select Top 1 Name_Sp From[NSI].[dbo].[Sp_NSI] Where Id_Nsi<'" + Id_Nsi + @"' and Name_Sp2 ='" + Combo + @"' and Name_Sp<>'')
                                    ,Sql_vyvod = (Select Top 1 Sql_vyvod From[NSI].[dbo].[Sp_NSI] Where Id_Nsi<'" + Id_Nsi + @"' and Name_Sp2 = '" + Combo + @"' and Sql_vyvod<>'')
                                    Where Id_Nsi = '" + Id_Nsi + @"'
                                    
                                    update [NSI].[dbo].[Sp_NSI] 
                                    Set Ot_Per2=(Select cast((cast(Ot_Per1 as Int)-1)as Char) as Ot_Per From [NSI].[dbo].[Sp_NSI] Where Id_Nsi='" + Id_Nsi + @"')
                                    Where Id_Nsi<'" + Id_Nsi + @"' and Name_Sp2='" + Combo + @"' and Ot_Per2='9999' and Name_Sp<>''";
                    ScriptGeneratorExec.ExecScript(sql3);
                    MessageBox.Show("Данные успешно скопированы", "Результат команды");
                }
            }
        }
    }
}  