using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Expert_VSR
{
    /// <summary>
    /// Логика взаимодействия для Prikriplenie.xaml
    /// </summary>
    public partial class Prikriplenie : Window
    {
        public ScriptGenerator ScriptGeneratorExec { get; set; }
        private List<Lpu_Pr> listLpu_Pr = new List<Lpu_Pr>();
        private List<DataPr> listDataPr = new List<DataPr>();
        private string TypeRst;
        public Prikriplenie()
        {
            InitializeComponent();
            ScriptGeneratorExec = new ScriptGenerator("Persist Security Info=False;User ID=sa;Password=sa;Initial Catalog=Oms_brn;Server=depo");
            Choice_RstType.Items.Add("Первичный");
            Choice_RstType.Items.Add("Повторный");
            Choice_RstType.Items.Add("Максимальный");
            Choice_RstType.SelectedIndex = 0;
        }
        private void Choice_RstType_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
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
            #region PR_search
            string queryString =
            @"Declare @Surname varchar(50) = '" + chooseSurname.Text.ToString() + @"'
            Declare @Name1 varchar(50) = '" + chooseName1.Text.ToString() + @"'
            Declare @Name2 varchar(50) = '" + chooseName2.Text.ToString() + @"'
            Declare @Birthday Char(10) = '" + chooseBirthday.Text.ToString() + @"'
            Declare @Enp varchar(20)= '" + chooseENP.Text.ToString() + @"'
            SELECT Distinct Top 50 isnull (Pe.Enp,'') As Enp
                  ,isnull (Surname,'') As Surname  ,isnull (Name1,'') As Name1 
                  ,isnull (Name2,'') As Name2 
                  ,isnull (substring (convert(Char,pe.Birthday,105),1,10),'') As Birthday 
                  ,h.[Dm] ,isnull (Lpu,'') As Lpu
                  ,isnull ((select top 1 name FROM [ExpertXml].[dbo].[lpu] l where l.ulcode=h.Lpu and ulCode<>''),'') As Name_LPU
                  ,isnull (substring (convert(Char,Date_In,105),1,10),'') As Date_In  
                  ,isnull (substring (convert(Char,Date_out,105),1,10),'') As Date_out
             FROM [Oms_brn].[dbo].[history] h
	            join [Oms_brn].[dbo].[Pers] Pe on h.IdPers=Pe.IdPers 
             WHERE pe.Surname Like @Surname+'%' and pe.Name1 Like @Name1+'%' and pe.Name2 Like @Name2+'%' and convert(Char,pe.Birthday,105) Like @Birthday+'%'
            and Pe.Enp Like @Enp+'%' 
            Order by Enp ,Surname ,Name1 ,Name2 ,Birthday ,Date_In Desc ,Date_out";
            #endregion
            listLpu_Pr.Clear();
            ScriptGeneratorExec.ExecSelect<Lpu_Pr>(queryString, out listLpu_Pr);
            DataGridLogView2.ItemsSource = listLpu_Pr;
        }

        void LoopVisualTree(DependencyObject obj)//обнуление текст боксов
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {

                if (obj is TextBox)
                {
                    ((TextBox)obj).Text = null;
                }
                // РЕКУРСИЯ
                LoopVisualTree(VisualTreeHelper.GetChild(obj, i));
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            LoopVisualTree(this);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (Ot_Per.Text.ToString().Contains("_"))
            {
                MessageBox.Show("Вы не заполнили Ot_Per", "Сообщение");
            }
            else
            {
                string sql = @"EXEC ExpertXml..VSR_Proverca " + Ot_Per.Text.ToString() + ',' + TypeRst.ToString();
                listDataPr.Clear();
                ScriptGeneratorExec.ExecSelect<DataPr>(sql, out listDataPr);
                DataGridLogView.ItemsSource = listDataPr;
            }
        }
    }
}
