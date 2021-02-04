using Microsoft.WindowsAPICodePack.Dialogs;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Expert_VSR
{
    /// <summary>
    /// Логика взаимодействия для Pisma.xaml
    /// </summary>
    public partial class Pisma : Window
    {
        public ScriptGenerator ScriptGeneratorExec { get; set; }
        public ScriptGenerator2 ScriptGeneratorExec2 { get; set; }
        private string TypeRst;


        public Pisma()
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
            var dlg = new CommonOpenFileDialog()
            {
                Title = "Выбор куда сохранить письма",
                IsFolderPicker = true
            };
            
            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                 string Lpu = "";
                //Формируем общую таблицу для расчетов ВЗР
                //Получаем список ЛПУ-плательщиков
                string sql_pisma = @"EXEC [ExpertXml].[dbo].[vsr_Pisma] '" + Ot_Per.Text.ToString() + "','" + TypeRst.ToString() + "','" + Lpu.ToString() + @"'";
                List<GetString> Name_File = new List<GetString>();
                ScriptGeneratorExec.ExecSelect(sql_pisma, out Name_File);

                //формируются уже отдельные файлы
                foreach (var n_file in Name_File)
                {
                    string template = "";
                    string Lpu_I="";
                    string Lpu_P = n_file.SqlString.Substring(3,6);
                    
                    if (n_file.SqlString.Length == 9)
                    {
                        template = Environment.CurrentDirectory + @"\Template\Pisma_Sum.xlsx";
                    }
                    else
                    {
                        template = Environment.CurrentDirectory + @"\Template\Pisma.xlsx";
                        Lpu_I = n_file.SqlString.Substring(10, 6);

                    }

                    #region SQL_pismaToEx
                    string sql_pismaToEx = @"use ExpertXml
                        Declare @Name_File Char(20)= '" + n_file.SqlString + @"'
                               ,@Ot_Per Char(4)='" + Ot_Per.Text.ToString() + @"'
		                       ,@TypeRst Char(2)='" + TypeRst.ToString() + @"'

                        If substring (@Name_File,1,3)='Ста'	and Len(@Name_File)<>'9'							---Выгружаем стационар
                        Begin

	                    Select 'За период: '+DATENAME(M,cast('20'+@Ot_Per+'01' as Date)) +' 20'+substring(@Ot_Per,1,2)+' ('+Case When @TypeRst='1' then 'Первичный' when @TypeRst='2' then 'Повторный' else 'Максимальный' end +')' info
		                    union all
	                    Select 'МО-плательщик: '+substring(@Name_File,4,6)+' '+(Select Top 1 Name From Lpu Where Ulcode=substring(@Name_File,4,6)) info
                 
                        -------Берем с Базы ранее сформированную таблицу
                        if object_id('tempdb..#StTbl1', 'U') is not null
                        drop table #StTbl1
                        Select * into #StTbl1
                        From [TestY].[dbo].[C_VSRTemp2]
                        Where Lpu_Code=substring(@Name_File,11,6) and Npr_Mo=substring (@Name_File,4,6)  
                                 
                        Select isnull(cast (n As Char),'') n,Polis ,Fio ,Npr_MO ,Code_Usl ,[Data] ,SumV 
                        From (Select cast (ROW_NUMBER() over (order by Fio) As int) as n
			                    ,Polis ,Fio ,Npr_MO ,Code_Usl ,[Data] ,cast (SumV As Char) SumV 
	                    From #StTbl1
	                        union All
	                    Select null n, 'Итого:' Polis ,'' Fio ,'' Npr_MO ,'' Code_Usl ,'' [Data] ,cast (Sum (SumV)As Char) SumV
	                    From #StTbl1) t
                        Order By -n Desc ,Fio
	
	                    Select info From [nsi].dbo.[Vsr_Podpis]
	                    union all 
	                    Select convert(Char(10),GetDate(),103)
                        end

                        If substring (@Name_File,1,3)='АПП'	and Len(@Name_File)<>'9'							--Выгружаем поликлинику
                        Begin
    
	                    Select 'За период: '+DATENAME(M,cast('20'+@Ot_Per+'01' as Date)) +' 20'+substring(@Ot_Per,1,2)+' ('+Case When @TypeRst='1' then 'Первичный' when @TypeRst='2' then 'Повторный' else 'Максимальный' end +')' info
		                    union all
	                    Select 'МО-плательщик: '+substring(@Name_File,4,6)+' '+(Select Top 1 Name From Lpu Where Ulcode=substring(@Name_File,4,6)) info

                        ----Берем с Базы ранее сформированную таблицу
                        if object_id('tempdb..#PolTbl2', 'U') is not null
                        drop table #PolTbl2
                 
                        Select * into #PolTbl2
                        From [TestY].[dbo].[C_VSRTemp]                     
                        Where Lpu_Code=substring(@Name_File,11,6) and Code_Pr=substring(@Name_File,4,6)  
                
                        Select isnull(cast (n As Char),'') n,Polis ,Fio ,Npr_MO ,Code_Usl ,[Data] ,SumV 
                        From (Select cast (ROW_NUMBER() over (order by Fio) As int) as n
			                    ,Polis ,Fio ,Npr_MO ,Code_Usl ,[Data] ,cast (SumV As Char) SumV 
	                    From #PolTbl2
	                    union All
	                    Select null n, 'Итого:' Polis ,'' Fio ,'' Npr_MO ,'' Code_Usl ,'' [Data] ,cast (Sum (SumV)As Char) SumV
	                    From #PolTbl2) t
                        Order By -n Desc,Fio
	
	                    Select info From [nsi].dbo.[Vsr_Podpis]
		                    union all 
	                    Select convert(Char(10),GetDate(),103)
	                    end

	                    If substring (@Name_File,1,3)='Ста' and Len(@Name_File)='9' 				--Выгружаем стационар Суммы
	                    Begin
						
	                    Select 'За период: '+DATENAME(M,cast('20'+@Ot_Per+'01' as Date)) +' 20'+substring(@Ot_Per,1,2)+' ('+Case When @TypeRst='1' then 'Первичный' when @TypeRst='2' then 'Повторный' else 'Максимальный' end +')' info
	                    union all
	                    Select 'МО-плательщик: '+substring(@Name_File,4,6)+' '+(Select Top 1 Name From Lpu Where Ulcode=substring(@Name_File,4,6)) info

	                    ------Берем с БД ранее сформированную таблицу
	                    if object_id('tempdb..#StTbl3', 'U') is not null
		                    drop table #StTbl3 
	                    Select * into #StTbl3
	                    From [TestY].[dbo].[C_VSRTemp2]
	                    Where Npr_Mo=substring(@Name_File,4,6) 
                            
	                    if object_id('tempdb..#StTbl31', 'U') is not null
		                    drop table #StTbl31
	                    Select cast(ROW_NUMBER() over (partition by Npr_Mo order by Npr_Mo desc) As Char) as n,
	                    Npr_Mo,Max(Name_Npr_Mo) Name_Npr_Mo, Lpu_Code,Max(Lpu_Name) Lpu_Name, Sum(SumV) SumV into #stTbl31
	                    From #stTbl3   
	                    Group By Npr_Mo,Lpu_Code 
             
	                    Select isnull(cast (n As Char),'') As n ,Lpu_Name ,SumV 
	                    From (Select cast (n as int) n, Npr_MO ,Name_Npr_Mo ,Lpu_Code ,Lpu_Name ,cast (SumV As Char) SumV From #stTbl31
			                    union All
			                    Select null n, '' Npr_Mo , ''Name_Npr_Mo ,'' Lpu_Code , 'Итого:' Lpu_Name ,cast (Sum (SumV)As Char) SumV
			                    From #stTbl31) t
	                    Order By -n Desc,Npr_Mo,Lpu_Code    
	
	                    Select info From [nsi].dbo.[Vsr_Podpis]
		                    union all 
	                    Select convert(Char(10),GetDate(),103)
	                    End

	                    If substring (@Name_File,1,3)='АПП' and Len(@Name_File)='9'							--Выгружаем поликлиниеку суммы
	                    Begin

	                    Select 'За период: '+DATENAME(M,cast('20'+@Ot_Per+'01' as Date)) +' 20'+substring(@Ot_Per,1,2)+' ('+Case When @TypeRst='1' then 'Первичный' when @TypeRst='2' then 'Повторный' else 'Максимальный' end +')' info
		                    union all
	                    Select 'МО-плательщик: '+substring(@Name_File,4,6)+' '+(Select Top 1 Name From Lpu Where Ulcode=substring(@Name_File,4,6)) info

                    ---Брать с таблице в БД которая ранее сформирована    
	                    if object_id('tempdb..#PolTbl4', 'U') is not null
                            drop table #PolTbl4
	                    Select Lpu_Name ,Lpu_Code ,Code_Lpu ,Name_Pr ,Code_Pr ,Npr_Mo ,SumV 
                        into #PolTbl4
                        From [TestY].[dbo].[C_VSRTemp]  
	                    Where Code_Pr=substring(@Name_File,4,6) 

                        if object_id('tempdb..#PolTbl41', 'U') is not null
                                drop table #PolTbl41
	                    Select cast(ROW_NUMBER() over (partition by Code_Pr order by Code_Pr desc) As Char) as n,
		                    Code_Pr,Max(Name_Pr) Name_Pr,Lpu_Code,Max(Lpu_Name) Lpu_Name,Sum(SumV) SumV into #PolTbl41
	                    From #PolTbl4   
	                    Group By Code_Pr,Lpu_Code 
		   
	                    Select isnull(cast (n As Char),'') As n ,Lpu_Name ,SumV  From 
                        (Select cast (n as int) n, Code_Pr As Npr_MO ,Name_Pr As Name_Npr_Mo ,Lpu_Code ,Lpu_Name ,cast (SumV As Char) SumV 
                        From #PolTbl41
                            union All
                        Select  null n, '' Npr_Mo , ''Name_Npr_Mo ,'' Lpu_Code , 'Итого:' Lpu_Name ,cast (Sum (SumV)As Char) SumV
                            From #PolTbl41) t
                        Order By -n Desc, Npr_Mo,Lpu_Code  
	
	                    Select info From [nsi].dbo.[Vsr_Podpis]
		                    union all 
	                    Select convert(Char(10),GetDate(),103)
                        End";
                    #endregion

                    List<object> data = new List<object>();
                    ScriptGeneratorExec2.ExecSelect(sql_pismaToEx, out data);

                    FileInfo t_info = new FileInfo(template);
                    ExcelPackage pck = new ExcelPackage(t_info);
                    ExpToExcel exp = new ExpToExcel(data, pck);

                    //Раскладываем данные по папкам
                    if (Directory.Exists(dlg.FileName + @"\" + Lpu_P) != true)
                        Directory.CreateDirectory(dlg.FileName + @"\" + Lpu_P); //Создание папок Плательщика

                    string SaveFilePath = dlg.FileName + @"\"+ @"\"+ Lpu_P + @"\" + n_file.SqlString+@"_"+ Ot_Per.Text.ToString()+@"_"+TypeRst.ToString()+".xlsx";
                    if (File.Exists(SaveFilePath))
                        File.Delete(SaveFilePath);
                    pck.SaveAs(new FileInfo(SaveFilePath));

                    if (Lpu_I !="")
                    {
                        if (Directory.Exists(dlg.FileName + @"\" + Lpu_I) != true)
                            Directory.CreateDirectory(dlg.FileName + @"\" + Lpu_I); //Создание папок Исполнителя

                        string SaveFilePath2 = dlg.FileName + @"\" + @"\" + Lpu_I + @"\" + n_file.SqlString+@"_"+ Ot_Per.Text.ToString() + @"_" + TypeRst.ToString() + ".xlsx";
                        if (File.Exists(SaveFilePath2))
                            File.Delete(SaveFilePath2);
                        pck.SaveAs(new FileInfo(SaveFilePath2));
                    }
                }

                    MessageBox.Show("Копирование выполнено", "Результат команды");
            }
            else MessageBox.Show("Ошибка копирования", "Результат команды");
            
        }
    }
}
