import clr
clr.AddReferenceToFileAndPath("MiRo.SimHexWorld.Engine.dll")
clr.AddReferenceToFileAndPath("TomShane.Neoforce.Controls.dll")

from MiRo.SimHexWorld.Engine.Misc import Provider

""" 
	ScienceWindow class handlers 
"""
class Window:
	"""
		init window
	"""
	def Initialize(self, parent):
		self.parent = parent

	"""
		click callback for tech selection
	"""
	def Tech_Click( self, window, sender, args ):
		tech = Provider.GetTech(sender.Name)

		if game.Human.Technologies.Contains(tech):
			return

		if game.Human.PossibleTechnologies.Contains(tech):
			game.Human.CurrentResearch = tech

	"""
		click callback for close button
	"""
	def Close_Click( self, window, sender, args ):
		window.Close()

window = Window()