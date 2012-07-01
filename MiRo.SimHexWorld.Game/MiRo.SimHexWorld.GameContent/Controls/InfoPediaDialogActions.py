"""
	Info Pedia Dialog
"""
class Window:
	"""
		init event handling
	"""
	def Initialize(self, parent):
		self.parent = parent

	"""
		handler for focus / content
	"""
	def Focus(self, focus):
		self.parent.GetControl("Title").Text = focus.Title

""" init instance """
window = Window()