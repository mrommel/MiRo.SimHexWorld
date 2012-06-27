class Window:
	def Initialize(self, parent):
		self.parent = parent

	def Focused(self, window, focus):
		self.parent.GetControl("Title").Text = focus.Title

window = Window()