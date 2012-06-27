""" 
	MapOptionDialog class handlers 
"""
class Window:
	"""
		init window
	"""
	def Initialize(self, parent):
		self.parent = parent

		self.UpdateCheckBoxes()

	def UpdateCheckBoxes( self ):
		self.parent.GetControl("HideRecommondations").Checked = config.HideRecommondations
		self.parent.GetControl("ResourceIcons").Checked = config.ResourceIcons
		self.parent.GetControl("YieldIcons").Checked = config.YieldIcons
		self.parent.GetControl("HexGrid").Checked = config.HexGrid
		self.parent.GetControl("UnitIcons").Checked = config.UnitIcons
		self.parent.GetControl("UnitPromotions").Checked = config.UnitPromotions

	def HideRecommondations_Checked( self, window, sender, args ):
		config.HideRecommondations = not config.HideRecommondations

		config.Save()

	def ResourceIcons_Checked( self, window, sender, args ):
		config.ResourceIcons = not config.ResourceIcons

		config.Save()

	def YieldIcons_Checked( self, window, sender, args ):
		config.YieldIcons = not config.YieldIcons

		config.Save()

	def HexGrid_Checked( self, window, sender, args ):
		config.HexGrid = not config.HexGrid

		config.Save()

	def UnitIcons_Checked( self, window, sender, args ):
		config.UnitIcons = not config.UnitIcons

		config.Save()

	def UnitPromotions_Checked( self, window, sender, args ):
		config.UnitPromotions = not config.UnitPromotions

		config.Save()

window = Window()