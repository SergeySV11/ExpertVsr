using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Expert_VSR
{
    public class DataVr
    {

        public DataVr()
        {
            
        }
        public virtual void Init(IDataRecord record)
        {
        }
    }
    public class GetVariables : DataVr
    {
        public Guid id_report { get; private set; }
        public int id_variable { get; private set; }
        public string variable { get; private set; }
        public string variableType { get; private set; }
        public string variableDefaultValue { get; set; }
        public string variableDescription { get; private set; }

        public GetVariables()
        {

        }
        public GetVariables(IDataRecord record)
        {
            Init(record);
        }
        public override void Init(IDataRecord record)
        {
            id_report = (Guid)record["id_report"];
            id_variable = (int)record["id_variable"];
            variable = (string)record["variable"];
            variableType = (string)record["variableType"];
            variableDefaultValue = (string)record["variableDefaultValue"];
            variableDescription = (string)record["variableDescription"];
        }
    }
    public class DataSumm : DataVr
    {
        public string Usl_Ok { get; private set; }
        public string Summa { get; private set; }
        public string Period { get; private set; }

        public DataSumm()
        {
            Usl_Ok = "";
        }

        public DataSumm(IDataRecord record)
        {
            Init(record);
        }
        public override void Init(IDataRecord record)
        {
            Usl_Ok = (string)record["usl_ok"];
            Summa = (string)record["Summa"];
            Period = (string)record["Period"];
        }
        public override string ToString()
        {
            return String.Format("{0} {1} {2}"
        , Usl_Ok
        , Summa
        , Period);
        }
    }
    public class GetString : DataVr
    {
        public string SqlString { get; private set; }

        public GetString()
        {
            SqlString = "";
        }
        public GetString(IDataRecord record)
        {
            this.Init(record);
        }
        public override void Init(IDataRecord record)
        {
            SqlString = (string)record["SqlString"];
        }
    }
    public class Tbl : DataVr
    {
        public string tb1  { get; private set; }
        public string tb2 { get; private set; }
        public string tb3 { get; private set; }
        public string tb4 { get; private set; }
        public string tb5 { get; private set; }
        public string tb6 { get; private set; }
        public string tb7 { get; private set; }

        public Tbl()
        {
            tb1 = "";
        }
        public Tbl(IDataRecord record)
        {
            this.Init(record);
        }
        public override void Init(IDataRecord record)
        {
            tb1 = (string)record["t1"];
            tb2 = (string)record["t2"];
            tb3 = (string)record["t3"];
            tb4 = (string)record["t4"];
            tb5 = (string)record["t5"];
            tb6 = (string)record["t6"];
            tb7 = (string)record["t7"];
        }
    }
    public class Lpu_Pr : DataVr
    {
        public string ЕНП { get; private set; }
        public string Фамилия { get; private set; }
        public string Имя { get; private set; }
        public string Отчество { get; private set; }
        public string ДР { get; private set; }
        public string ЛПУ { get; private set; }
        public string Наименование_ЛПУ { get; private set; }
        public string Дата_1 { get; private set; }
        public string Дата_2 { get; private set; }

        public Lpu_Pr()
        {
            Фамилия = "";
        }
        public Lpu_Pr(IDataRecord record)
        {
            this.Init(record);
        }
        public override void Init(IDataRecord record)
        {
            ЕНП = (string)record["Enp"];
            Фамилия = (string)record["Surname"];
            Имя = (string)record["Name1"];
            Отчество = (string)record["Name2"];
            ДР = (string)record["Birthday"];
            ЛПУ = (string)record["Lpu"];
            Наименование_ЛПУ = (string)record["Name_Lpu"];
            Дата_1 = (string)record["Date_In"];
            Дата_2 = (string)record["Date_Out"];
        }

    }
    public class DataPr : DataVr
    {
        public string IdPers { get; private set; }
        public string Lpu { get; private set; }
        public string Name_Lpu { get; private set; }
        public string Npr_Mo { get; private set; }
        public string Fio { get; private set; }
        public string Enp { get; private set; }
        public string Snils { get; private set; }
        public string Phone { get; private set; }
        public string Addres { get; private set; }
        public string AdresFactich { get; private set; }
       // public string Sumv_Usl { get; private set; }

        public DataPr()
        {
            Lpu = "";
        }
        public DataPr(IDataRecord record)
        {
            this.Init(record);
        }
        public override void Init(IDataRecord record)
        {
            IdPers = (string)record["IdPers"];
            Lpu = (string)record["Lpu"];
            Name_Lpu = (string)record["Name_Lpu"];
            Npr_Mo = (string)record["Npr_Mo"];
            Fio = (string)record["FIO"];
            Enp = (string)record["Enp"];
            Snils = (string)record["Snils"];
            Phone = (string)record["Phone"];
            Addres = (string)record["Addres"];
            AdresFactich = (string)record["AdresFactich"];
           // Sumv_Usl = (string)record["Sumv_Usl"];

        }
    }
}

