import clr
clr.AddReferenceToFileAndPath("MiRo.SimHexWorld.Engine.dll")

from MiRo.SimHexWorld.Engine.Misc import Provider
from MiRo.SimHexWorld.Engine.Instance import GameFacade, GameNotification

"""
	Dialog handlers for Map Load Dialog
"""
class Window:
	"""
		init events
	"""
	def Initialize(self, parent):
		self.parent = parent

		self.parent.GetControl("Maps").Items.AddRange(Provider.Instance.Maps.Values)

	""" 
		user selected a map -> send notification to facade, then close
	"""
	def Select_Click(self, window, sender, args ):
		item = window.GetControl("Maps").Items[ window.GetControl("Maps").ItemIndex]
		GameFacade.getInstance().SendNotification(GameNotification.LoadMap, item)
		window.Close()

	"""
		user canceled, close
	"""
	def Close_Click(self, window, sender, args ):
		window.Close()

""" create instance """
window = Window()