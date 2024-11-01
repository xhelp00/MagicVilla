using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers;

[ApiController]
[Route("api/VillaAPI")]
public class VillaApiController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<VillaDTO>> GetVillas()
    {
        return Ok(VillaStore.villaList);
    }

    [HttpGet("{id:int}", Name = "GetVilla")]
    /*[ProducesResponseType(200, Type = typeof(VillaDTO))]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]*/
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VillaDTO))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]

    public ActionResult GetVilla(int id)
    {
        if (id == 0)
        {
            return BadRequest();
        }
        var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
        if (villa == null)
        {
            return NotFound();
        }
        return Ok(villa);
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(VillaDTO))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<VillaDTO> CreateVilla([FromBody]VillaDTO villaDTO)
    {
        /*if (ModelState.IsValid == false)
        {
            return BadRequest(ModelState);
        }*/
        if (VillaStore.villaList.FirstOrDefault(u=>u.Name.ToLower() == villaDTO.Name.ToLower()) != null)
        {
            ModelState.AddModelError("CustomError", "Villa already exists");
            return BadRequest(ModelState);
        }
        if (villaDTO == null)
        {
            return BadRequest(villaDTO);
        }
        if (villaDTO.Id > 0)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
        villaDTO.Id = VillaStore.villaList.Max(v => v.Id) + 1;
        VillaStore.villaList.Add(villaDTO);
        
        return CreatedAtRoute("GetVilla", new { id = villaDTO.Id }, villaDTO);
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpDelete("{id:int}", Name = "DeleteVilla")]
    public IActionResult DeleteVilla(int id)
    {
        if(id == 0)
        {
            return BadRequest();
        }
        var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
        if(villa == null)
        {
            return NotFound();
        }
        VillaStore.villaList.Remove(villa);
        return NoContent();
    }
    
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPut("{id:int}", Name = "UpdateVilla")]
    public IActionResult UpdateVilla(int id, [FromBody] VillaDTO villaDTO)
    {
        if (id == 0)
        {
            return BadRequest();
        }
        if (villaDTO == null)
        {
            return BadRequest(villaDTO);
        }
        if (villaDTO.Id != id)
        {
            return BadRequest();
        }
        var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
        if (villa == null)
        {
            return NotFound();
        }
        villa.Name = villaDTO.Name;
        villa.Occupancy = villaDTO.Occupancy;
        villa.Sqft = villaDTO.Sqft;
        return NoContent();
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
    public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDTO> patchDTO)
    {
        if (id == 0)
        {
            return BadRequest();
        }

        if (patchDTO == null)
        {
            return BadRequest();
        }

        var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
        if (villa == null)
        {
            return NotFound();
        }

        patchDTO.ApplyTo(villa, ModelState);
        return NoContent();
    }

}

