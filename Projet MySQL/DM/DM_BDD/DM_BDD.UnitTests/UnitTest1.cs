using System;
using DM_BDD;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DM_BDD
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Test_Client_Modifier_Tel()
        {
            Program.CoSQL co = new Program.CoSQL("root");
            Program.Client c1 = new Program.Client("Mario", co);
            string tel = "1111030405";
            c1.Modifier_Tel(tel, co);
            Assert.AreEqual(c1.Client_tel, tel);
        }

        [TestMethod]
        public void Test_Client_Modifier_Prix()
        {
            Program.CoSQL co = new Program.CoSQL("root");
            Program.Recette r = new Program.Recette("Spaghetti Bolognaise", co);
            float nouvprix = 300;
            r.Modifier_prix(nouvprix, co);
            Assert.AreEqual(r.Recette_prix, nouvprix);
        }
    }
}
