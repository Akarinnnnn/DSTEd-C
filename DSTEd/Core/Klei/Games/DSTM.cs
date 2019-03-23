namespace DSTEd.Core.Klei.Games {
    class DSTM : KleiGame {
        public DSTM() : base() {
            this.id = 245850;
            this.name = I18N.__("Mod Tools");

            this.AddTool("DST_TOOLS", null);
            this.AddSubTool("DST_TOOLS", "FMOD_DESIGNER", "mod_tools/FMOD_Designer/fmod_designer.exe");
            this.AddSubTool("DST_TOOLS", "SPRITER", "mod_tools/Spriter/Spriter.exe");
            this.AddSubTool("DST_TOOLS", "TILED", "mod_tools/Tiled/tiled.exe");
            this.AddSubTool("DST_TOOLS", "TEXTURE_VIEWER", "mod_tools/tools/bin/TextureViewer.exe");//we have KTEX editor instead
			//AddSubTool("DST_TOOLS", "AutoCompiler", "mod_tools/autocompiler.exe");
		   /*
			* autocompiler compiles textures and anims automactually
			* 
			* autocompiler -> png -> image_build.py (image_build.py "image_path"), we can use KTEX class instead
			*		|
			*	 	 --------> scml(some dealing on C++ side) -> zipanim.py "temp dir" "output zip",
			*	 												 export.py --skip_update_prefabs --outputdir "output dir" --prefabsdir "output zip dir"
			* 
			*  png "input png" "useless argument"
			* scml "input scml" "output zip"
			* 
            * @ToDo add scripts with specific arguments/parameters (see these secripts, check their help files & arguments)
            * compiler_scripts/image_build.py   //image_build.py "image_path"
            * compiler_scripts/properties.py
            * compiler_scripts/properties.pyc
            * compiler_scripts/zipanim.py       //zipanim.py "scml output temp dir" "output temp zip"
            * 
            * mod_tools/export.py
            * mod_tools/ExportOptions.py
            * mod_tools/ResizeInfo.py           //export.py --skip_update_prefabs --outputdir "output anim folder dir" --prefabsdir "temp zip file dir(from zipanim.py)"
            * mod_tools/validate.py
            *
            * scripts/ds_to_spriter.py
            * scripts/resize.py
            * 
            * tools/bin/ShaderCompiler.exe
            * tools/bin/TextureConverter.exe
            */
		}
	}
}
