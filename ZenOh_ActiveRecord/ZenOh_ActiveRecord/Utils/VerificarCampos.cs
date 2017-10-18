using System;
using System.Text;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ZenOh_ActiveRecord.Utils
{
    public class ValidarCampos<T>
    {
        private StringBuilder CamposRequeridosNaoPreenchidos;

        public ValidarCampos()
        {
            CamposRequeridosNaoPreenchidos = new StringBuilder();

        }

        public void ValidarPreenchimentoDeCamposRequeridos(List<T> ListaDeEntidade)
        {
            foreach (var i in ListaDeEntidade)
            {
                this.ValidarPreenchimentoDeCamposRequeridos(i);
            }
        }

        public void ValidarPreenchimentoDeCamposRequeridos(T Entidade)
        {
            var PossuiCampoRequeridoNaoPreenchido = false;

            Type TipoEntidade = Entidade.GetType();

            foreach (var Propriedade in TipoEntidade.GetTypeInfo().GetProperties())
            {
                var DisplayName = "";
                var CampoERequeridoENaoEstaPreenchido = false;

                foreach (var Atributo in Propriedade.GetCustomAttributes())
                {

                    if (Atributo.GetType() == typeof(DisplayAttribute))
                    {
                        DisplayName = ((DisplayAttribute)Atributo).Name;
                    }


                    try
                    {
                        if ((!CamposRequeridosNaoPreenchidos.ToString().Contains(DisplayName)) &&
                            (Atributo.GetType() == typeof(RequiredAttribute)) &&
                            (Propriedade.GetValue(Entidade).Equals("")))
                        {
                            CampoERequeridoENaoEstaPreenchido = true;
                        }
                    }
                    catch (NullReferenceException)
                    {
                        CampoERequeridoENaoEstaPreenchido = true;
                    }

                    if (CampoERequeridoENaoEstaPreenchido)
                    {
                        CamposRequeridosNaoPreenchidos.AppendLine(DisplayName);
                        PossuiCampoRequeridoNaoPreenchido = true;
                    }
                }

            }

            if (PossuiCampoRequeridoNaoPreenchido)
                throw new Exception("As seguintes informações devem ser preenchidas:\n" + CamposRequeridosNaoPreenchidos.ToString());

        }
    }
}
