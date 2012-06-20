from System.Collections.Generic import List

import clr
clr.AddReferenceToFileAndPath("MiRo.SimHexWorld.Engine.dll")
clr.AddReferenceToFileAndPath("TomShane.Neoforce.Controls")

from MiRo.SimHexWorld.Engine.Types import UnitAction
from TomShane.Neoforce.Controls import ImageBox

class Window:
	""" MainWindow class handlers """
	def Initialize(self, parent):
		self.parent = parent

	def FocusChanged(self, window, mapChangeArgs):
		if (mapChangeArgs.Map == None):
			return
		self.parent.Focus = mapChangeArgs.UpdatedTiles[0]
		return

	def CityOpened(self, window, city):
		pass

	def CitySelected(self, window, city):
		pass

	def UnitUnselected(self, window):
		window.CurrentUnit = None

		window.GetControl("UnitAction0").Visible = False
		window.GetControl("UnitAction1").Visible = False

	def HumanUnitSelected( self, window, unit):
		window.CurrentUnit = unit
		window.CurrentUnitActions = List[UnitAction]()

		i = 0
		for action in unit.Actions:
			window.CurrentUnitActions.Add(action)
		
			actionButton = window.GetControl("UnitAction" + str(i))
			
			if actionButton != None:
				actionButton.Visible = True
			i = i + 1

		return

	def ActionButton_Click( self, window, sender, args ):
		if window.CurrentUnitActions == None:
			return
		
		num = int(sender.Name[-1])	
		action = window.CurrentUnitActions[num]

		if window.CurrentUnit != None:
			window.CurrentUnit.Execute(action)
			args.Handled = True

	def Unit_Click( self, window, sender, args ):
		pass

	def BtnAdvisors_Click( self, window, sender, args ):
		pass

	def MapOptions_Click( self, window, sender, args ):
		""" toggle map option view """
		window.MapOptions.Visible = not window.MapOptions.Visible;

	def LblResearch_Click( self, window, sender, args ):
		""" if( Game.Human.CurrentResearch != None )
			ScienceInfoDialog.Show(Manager, Game.Human.CurrentResearch, "Science");"""
		pass

	def Science_Click( self, window, sender, args ):
		window.ScienceDialog.ShowModal()

	def BtnPolicies_Click( self, window, sender, args ):
		window.PolicyChooseDialog.ShowModal()

	def BtnDiplomacy_Click( self, window, sender, args ):
		window.DiplomacyDialog.ShowModal()

window = Window()