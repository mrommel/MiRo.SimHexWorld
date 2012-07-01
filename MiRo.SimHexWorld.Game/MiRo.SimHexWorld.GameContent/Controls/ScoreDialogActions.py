import clr
clr.AddReferenceToFileAndPath("MiRo.SimHexWorld.Engine.dll")

from MiRo.SimHexWorld.Engine.Misc import Provider
from MiRo.SimHexWorld.Engine.Instance import GameFacade, GameNotification

"""
	Dialog handlers for Score Dialog
"""
class Window:
	"""
		init events
	"""
	def Initialize(self, parent):
		self.parent = parent

	"""
		user canceled, close
	"""
	def Close_Click(self, window, sender, args ):
		window.Close()

""" create instance """
window = Window()