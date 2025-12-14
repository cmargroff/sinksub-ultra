@tool

class_name PhysicsLayersEnumPlugin
extends EditorPlugin

var SecitonKey = "layer_names"

var SectionRegex: RegEx

func _init():
  SectionRegex = RegEx.new()
  SectionRegex.compile("^([^/]+)/layer_(\\d+)")

func _enter_tree():
  ProjectSettings.connect("settings_changed", Generate)
  pass

func _exit_tree():
  pass

func Generate():
  var projectFileContents: String = FileAccess.get_file_as_string("res://project.godot")
  var settings: ConfigFile = ConfigFile.new()
  var err: Error = settings.parse(projectFileContents)
  if(err != Error.OK):
      printerr("Failed to parse project.godot")
  
  var csOutput: String = "// This is a generated file, do not commit.\n\npublic enum PhysicsLayers {\n"
  var gdOutput: String = "# This is a generated file, do not commit.\n\nenum PhysicsLayers {"
  
  var layers: PackedStringArray = settings.get_section_keys(SecitonKey)
  
  for key in layers:
    var match = SectionRegex.search(key)
    if (match):
      var group = match.get_string(1)
      var index: int = match.get_string(2).to_int()
      var name = settings.get_value(SecitonKey, key)
      csOutput += "  %s = 1 << %d,\n" % [name, index - 1]
      gdOutput += "%s = 1 << %d, " % [name, index - 1]
    pass
  csOutput += "}"
  gdOutput += "}"
  var csFile = FileAccess.open("res://addons/physics_layers_enum/PhysicsLayersEnum.g.cs", FileAccess.WRITE)
  csFile.store_string(csOutput)
  csFile.close()
  var gdFile = FileAccess.open("res://addons/physics_layers_enum/PhysicsLayersEnum.g.gd", FileAccess.WRITE)
  gdFile.store_string(gdOutput)
  gdFile.close()
  pass
