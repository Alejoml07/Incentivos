using System;
namespace PuntosLeonisa.Products.Infrasctructure.Common.Communication
{
	public class GenericResponse<T> 
	{
        public bool IsSuccess { get; set; } 
        public string Message { get; set; } = null!;
        public T Result { get; set; } 

        public GenericResponse()
        {
            this.Message = "Proceso realizado correctamente";
            this.IsSuccess = true;
        }
    }
}

