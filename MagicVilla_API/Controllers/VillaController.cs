﻿using MagicVilla_API.Datos;
using MagicVilla_API.Modelos;
using MagicVilla_API.Modelos.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace MagicVilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaController : ControllerBase
    {


        private readonly AplicationDbcontext? _db;

        public VillaController(AplicationDbcontext db)
        {
            _db = db;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]      
        public ActionResult<IEnumerable<VillaDto>> GetVillas()  //ActionResult se agrego para el tipo de retorno
        {
            return Ok(_db.Villas.ToList());  // es un select a la bd

        }
        [HttpGet("id:int", Name = "GetVilla")]  // se agrego el name para usarlo en el post
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<VillaDto> GetVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            var villa = _db.Villas.FirstOrDefault(v => v.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            return Ok(villa);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<VillaDto> CrearVilla([FromBody] VillaDto villaDto)
        {
            if (!ModelState.IsValid) //Vaidando el modelo
            {
                return BadRequest(ModelState);
            }
            if (villaDto == null)
            {
                return BadRequest(villaDto);
            }
            if (_db.Villas.FirstOrDefault(v => v.Nombre.ToLower() == villaDto.Nombre.ToLower()) != null) //Valdacion personalizada
            {
                ModelState.AddModelError("NombreExiste", "La villa con ese nombre ya existe");
                return BadRequest(ModelState);
            }


            if (villaDto.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
           // villaDto.Id = VillaStore.villaList.OrderByDescending(v => v.Id).First().Id + 1;
            //VillaStore.villaList.Add(villaDto);
            //return Ok(villaDto); //****** este es valido pero lo cmabiaremos para traer toda la lista de la API
            
            // se crea un modelo de tipo villa
             Villa modelo= new ()
             { 
               Nombre= villaDto.Nombre,
               Detalle= villaDto.Detalle,
               ImagenUrl= villaDto.ImagenUrl,
               Ocupantes= villaDto.Ocupantes,
               Tarifa= villaDto.Tarifa,
               MetrosCuadrados=villaDto.MetrosCuadrados,
               Amenidad= villaDto.Amenidad
             
             };

            _db.Add(modelo);  // es un insert a la bd
            _db.SaveChanges();
            
            return CreatedAtRoute("GetVilla", new { id = villaDto.Id }, villaDto);
        }

        [HttpDelete("id:int")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public IActionResult DeleteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var villa = _db.Villas.FirstOrDefault(v => v.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
           // VillaStore.villaList.Remove(villa);
           _db.Remove(villa);
            _db.SaveChanges();
            return NoContent();
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public IActionResult UpdateVilla( int id,[FromBody] VillaDto villaDto )
        {
            if (villaDto==null || id!=villaDto.Id  )
            {
                return BadRequest();
            }
            //var villa = _db.Villas.FirstOrDefault(v => v.Id == id);

            //villa.Nombre= villaDto.Nombre;
            //villa.Ocupantes= villaDto.Ocupantes;
            //villa.MetrosCuadrados = villaDto.MetrosCuadrados;
            Villa modelo = new ()
            {
                Id = villaDto.Id,
                Nombre = villaDto.Nombre,
                Detalle = villaDto.Detalle,
                ImagenUrl = villaDto.ImagenUrl,
                Ocupantes = villaDto.Ocupantes,
                Tarifa = villaDto.Tarifa,
                MetrosCuadrados = villaDto.MetrosCuadrados,
                Amenidad = villaDto.Amenidad

            };
            _db.Villas.Update(modelo);
            _db.SaveChanges();
            return NoContent();

        }


        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDto> PatchDto) // se agrega esto por las librerias nugget jsonpathc
        {
            if (PatchDto == null || id == 0)
            {
                return BadRequest();
            }
            var villa = _db.Villas.AsNoTracking().FirstOrDefault(v => v.Id == id); // Asnotracking es para consultar y no se trackea con Villa y villaDto, cuando trabajamos con un registro que despus volvemos a instanciar
            VillaDto villaDto = new ()
            {
                Id = villa.Id,
                Nombre = villa.Nombre,
                Detalle = villa.Detalle,
                ImagenUrl = villa.ImagenUrl,
                Ocupantes = villa.Ocupantes,
                Tarifa = villa.Tarifa,
                MetrosCuadrados = villa.MetrosCuadrados,
                Amenidad = villa.Amenidad

            };

            if (villa == null) return BadRequest();
            
            PatchDto.ApplyTo(villaDto, ModelState);
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Villa modelo = new ()
            {
                Id = villaDto.Id,
                Nombre = villaDto.Nombre,
                Detalle = villaDto.Detalle,
                ImagenUrl = villaDto.ImagenUrl,
                Ocupantes = villaDto.Ocupantes,
                Tarifa = villaDto.Tarifa,
                MetrosCuadrados = villaDto.MetrosCuadrados,
                Amenidad = villaDto.Amenidad

            };
            _db.Villas.Update(modelo);
            _db.SaveChanges();

            return NoContent();

        }

    }
}
